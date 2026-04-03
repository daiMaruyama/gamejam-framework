using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameJamCore;
using GameJamScene;
using UnityEngine;
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
		/// <summary>オープニング演出の CanvasGroup。省略するとアイドル演出をスキップする。</summary>
		[SerializeField] private CanvasGroup _openingCanvasGroup;

		/// <summary>フェードイン/アウトの時間（秒）。</summary>
		[SerializeField] private float _fadeDuration = 0.5f;

		[Header("シーン遷移")]
		/// <summary>ゲーム開始時に遷移するシーン名。</summary>
		[SerializeField] private string _nextSceneName = "Game";

		private float _idleTimer;
		private bool _isOpeningPlaying;
		private bool _isTransitioning;

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
			if (_isTransitioning)
			{
				return;
			}

			if (Input.anyKeyDown)
			{
				OnInput();
				return;
			}

			if (_isOpeningPlaying)
			{
				return;
			}

			_idleTimer += Time.deltaTime;

			if (_idleTimer >= _idleTimeLimit)
			{
				PlayOpeningAsync().Forget();
			}
		}

		private void OnInput()
		{
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

			if (_openingCanvasGroup == null)
			{
				return;
			}

			_openingCanvasGroup.gameObject.SetActive(true);
			await _openingCanvasGroup.DOFade(1f, _fadeDuration).SetUpdate(true).ToUniTask();
		}

		/// <summary>
		/// オープニング演出をフェードアウトで停止する。
		/// </summary>
		private async UniTaskVoid StopOpeningAsync()
		{
			if (_openingCanvasGroup == null)
			{
				_isOpeningPlaying = false;
				return;
			}

			await _openingCanvasGroup.DOFade(0f, _fadeDuration).SetUpdate(true).ToUniTask();
			_openingCanvasGroup.gameObject.SetActive(false);
			_isOpeningPlaying = false;
		}

		/// <summary>
		/// ゲームシーンへ遷移する。ISceneService があればトランジション付きで遷移する。
		/// </summary>
		private async UniTaskVoid StartGameAsync()
		{
			_isTransitioning = true;

			if (ServiceLocator.TryGet<ISceneService>(out var scene))
			{
				await scene.LoadAsync(_nextSceneName);
			}
			else
			{
				SceneManager.LoadScene(_nextSceneName);
			}
		}
	}
}
