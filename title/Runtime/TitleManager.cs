using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameJamCore;
using GameJamScene;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GameJamTitle
{
	/// <summary>
	/// タイトル画面の基盤コンポーネント。
	/// アイドルタイマー・オープニング演出・シーン遷移をまとめて管理する。
	/// タイトルシーンの GameObject にアタッチして使う。
	/// </summary>
	public class TitleManager : MonoBehaviour
	{
		[Header("アイドル設定")]
		/// <summary>この秒数操作がないとオープニング演出を再生する。</summary>
		[SerializeField] private float _idleTimeLimit = 10f;

		[Header("オープニング演出")]
		/// <summary>オープニング演出の CanvasGroup。省略するとアイドル後も通常待機を継続する。</summary>
		[SerializeField] private CanvasGroup _openingCanvasGroup;

		/// <summary>フェードイン/アウトの時間（秒）。</summary>
		[SerializeField] private float _fadeDuration = 0.5f;

		[Header("シーン遷移")]
		/// <summary>ゲーム開始時に遷移するシーン名。</summary>
		[SerializeField] private string _nextSceneName = "InGame";

		[Header("入力設定")]
		/// <summary>
		/// ゲーム開始・オープニングキャンセルに使う InputAction。
		/// 未アサインの場合はキーボード Space・Enter・ゲームパッド South を使う。
		/// </summary>
		[SerializeField] private InputActionReference _startAction;

		private float _idleTimer;
		private bool _isOpeningPlaying;
		private bool _isTransitioning;
		private bool _isStoppingOpening;
		private InputAction _action;

		private void OnEnable()
		{
			if (_startAction != null)
			{
				_action = _startAction.action;
			}
			else
			{
				_action = new InputAction();
				_action.AddBinding("<Keyboard>/space");
				_action.AddBinding("<Keyboard>/enter");
				_action.AddBinding("<Gamepad>/buttonSouth");
			}

			_action.performed += _ => OnInput();
			_action.Enable();
		}

		private void OnDisable()
		{
			_action.performed -= _ => OnInput();
			_action.Disable();

			if (_startAction == null)
			{
				_action.Dispose();
			}
		}

		private void Start()
		{
			if (_openingCanvasGroup != null)
			{
				_openingCanvasGroup.alpha = 0f;
				_openingCanvasGroup.gameObject.SetActive(false);
			}
		}

		private void Update()
		{
			if (_isTransitioning || _isOpeningPlaying)
			{
				return;
			}

			_idleTimer += Time.deltaTime;

			if (_idleTimer >= _idleTimeLimit)
			{
				_idleTimer = 0f;

				if (_openingCanvasGroup != null)
				{
					PlayOpeningAsync().Forget();
				}
			}
		}

		private void OnInput()
		{
			if (_isTransitioning || _isStoppingOpening)
			{
				return;
			}

			_idleTimer = 0f;

			if (_isOpeningPlaying)
			{
				StopOpeningAsync().Forget();
			}
			else
			{
				StartGameAsync().Forget();
			}
		}

		/// <summary>
		/// オープニング演出をフェードインで再生する。
		/// </summary>
		private async UniTaskVoid PlayOpeningAsync()
		{
			_isOpeningPlaying = true;
			_openingCanvasGroup.gameObject.SetActive(true);
			await _openingCanvasGroup.DOFade(1f, _fadeDuration).SetUpdate(true).ToUniTask();
		}

		/// <summary>
		/// オープニング演出をフェードアウトで停止する。
		/// </summary>
		private async UniTaskVoid StopOpeningAsync()
		{
			_isStoppingOpening = true;
			await _openingCanvasGroup.DOFade(0f, _fadeDuration).SetUpdate(true).ToUniTask();
			_openingCanvasGroup.gameObject.SetActive(false);
			_isOpeningPlaying = false;
			_isStoppingOpening = false;
		}

		/// <summary>
		/// ゲームシーンへ遷移する。ISceneService があればトランジション付きで遷移する。
		/// </summary>
		private async UniTaskVoid StartGameAsync()
		{
			_isTransitioning = true;

			try
			{
				if (ServiceLocator.TryGet<ISceneService>(out var scene))
				{
					await scene.LoadAsync(_nextSceneName);
				}
				else
				{
					SceneManager.LoadScene(_nextSceneName);
				}
			}
			finally
			{
				_isTransitioning = false;
			}
		}
	}
}
