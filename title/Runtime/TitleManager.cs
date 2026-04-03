using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GameJamTitle
{
	/// <summary>
	/// タイトル画面の基盤コンポーネント。
	/// アイドルタイマーとオープニング演出を管理し、スタート入力時に UnityEvent を発火する。
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

		[Header("イベント")]
		/// <summary>スタート入力時に発火するイベント。シーン遷移など外部処理を接続する。</summary>
		[SerializeField] private UnityEvent _onStartRequested;

		[Header("入力設定")]
		/// <summary>
		/// ゲーム開始・オープニングキャンセルに使う InputAction。
		/// 未アサインの場合はキーボード Space・Enter・ゲームパッド South を使う。
		/// </summary>
		[SerializeField] private InputActionReference _startAction;

		private float _idleTimer;
		private bool _isOpeningPlaying;
		private bool _isStoppingOpening;
		private bool _hasStarted;
		private InputAction _action;
		private Action<InputAction.CallbackContext> _onPerformed;

		private void OnEnable()
		{
			_hasStarted = false;

			if (_startAction != null)
			{
				_action = _startAction.action;
				if (_action == null)
				{
					return;
				}
			}
			else
			{
				_action = new InputAction();
				_action.AddBinding("<Keyboard>/space");
				_action.AddBinding("<Keyboard>/enter");
				_action.AddBinding("<Gamepad>/buttonSouth");
			}

			_onPerformed = _ => OnInput();
			_action.performed += _onPerformed;
			_action.Enable();
		}

		private void OnDisable()
		{
			if (_action == null)
			{
				return;
			}

			_action.performed -= _onPerformed;
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
			if (_isOpeningPlaying)
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
			if (_isStoppingOpening || _hasStarted)
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
				_hasStarted = true;
				_onStartRequested?.Invoke();
			}
		}

		/// <summary>
		/// オープニング演出をフェードインで再生する。
		/// </summary>
		private async UniTaskVoid PlayOpeningAsync()
		{
			_isOpeningPlaying = true;
			_openingCanvasGroup.gameObject.SetActive(true);
			_openingCanvasGroup.DOKill();
			await _openingCanvasGroup.DOFade(1f, _fadeDuration).SetUpdate(true).ToUniTask();
		}

		/// <summary>
		/// オープニング演出をフェードアウトで停止する。
		/// </summary>
		private async UniTaskVoid StopOpeningAsync()
		{
			_isStoppingOpening = true;
			_openingCanvasGroup.DOKill();
			await _openingCanvasGroup.DOFade(0f, _fadeDuration).SetUpdate(true).ToUniTask();
			_openingCanvasGroup.gameObject.SetActive(false);
			_isOpeningPlaying = false;
			_isStoppingOpening = false;
		}
	}
}
