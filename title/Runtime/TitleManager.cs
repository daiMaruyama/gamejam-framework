using System;
using System.Threading;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

namespace GameJamTitle
{
	/// <summary>
	/// タイトル画面の基盤コンポーネント。
	/// アイドル一定時間で動画を再生し、入力で止める。
	/// タイトルシーンの GameObject にアタッチして使う。
	/// </summary>
	public class TitleManager : MonoBehaviour
	{
		[Header("アイドル設定")]
		/// <summary>この秒数操作がないと動画を再生する。</summary>
		[SerializeField] private float _idleTimeLimit = 10f;

		[Header("動画")]
		/// <summary>アイドル時に再生する VideoPlayer。</summary>
		[SerializeField] private VideoPlayer _videoPlayer;

		[Header("フェード（任意）")]
		/// <summary>動画の前後にフェードをかける CanvasGroup。省略するとフェードなしで即再生。</summary>
		[SerializeField] private CanvasGroup _fadeCanvasGroup;

		/// <summary>フェードイン/アウトの時間（秒）。</summary>
		[SerializeField] private float _fadeDuration = 0.5f;

		[Header("動画再生中に非表示にするUI")]
		/// <summary>動画再生中に非表示にする GameObject。停止時に再表示する。</summary>
		[SerializeField] private GameObject[] _hideOnPlay;

		[Header("入力設定")]
		/// <summary>
		/// 動画スキップに使う InputAction。
		/// 未アサインの場合はマウス左クリック・キーボード Space・Enter・ゲームパッド South を使う。
		/// </summary>
		[SerializeField] private InputActionReference _skipAction;

		private float _idleTimer;
		private bool _isPlaying;
		private int _playGeneration;
		private InputAction _action;
		private Action<InputAction.CallbackContext> _onPerformed;
		private VideoPlayer.EventHandler _onLoopPointReached;
		private bool[] _hideOnPlayCache;

		private void OnEnable()
		{
			if (_videoPlayer != null)
			{
				_onLoopPointReached = _ => StopVideo();
				_videoPlayer.loopPointReached += _onLoopPointReached;
			}

			if (_skipAction != null)
			{
				_action = _skipAction.action;
				if (_action == null)
				{
					Debug.LogError($"[TitleManager] {_skipAction.name} の action が null です。InputActionAsset を確認してください。");
					return;
				}
			}
			else
			{
				_action = new InputAction();
				_action.AddBinding("<Mouse>/leftButton");
				_action.AddBinding("<Keyboard>/space");
				_action.AddBinding("<Keyboard>/enter");
				_action.AddBinding("<Gamepad>/buttonSouth");
			}

			_onPerformed = _ => OnSkip();
			_action.performed += _onPerformed;
			_action.Enable();
		}

		private void OnDisable()
		{
			StopVideo();

			if (_videoPlayer != null)
			{
				_videoPlayer.loopPointReached -= _onLoopPointReached;
			}

			if (_action == null)
			{
				return;
			}

			_action.performed -= _onPerformed;
			_action.Disable();

			if (_skipAction == null)
			{
				_action.Dispose();
			}
		}

		private void Start()
		{
			if (_fadeCanvasGroup != null)
			{
				_fadeCanvasGroup.alpha = 0f;
				_fadeCanvasGroup.blocksRaycasts = false;
				_fadeCanvasGroup.interactable = false;
			}
		}

		private void Update()
		{
			if (_isPlaying) return;

			_idleTimer += Time.deltaTime;

			if (_idleTimer >= _idleTimeLimit)
			{
				_idleTimer = 0f;
				PlayAsync();
			}
		}

		/// <summary>
		/// アイドルタイマーをリセットする。外部UIなどから呼び出して操作中の動画再生を防ぐ。
		/// </summary>
		public void NotifyActivity()
		{
			_idleTimer = 0f;
		}

		private void OnSkip()
		{
			_idleTimer = 0f;

			if (!_isPlaying) return;

			StopVideo();
		}

		private async void PlayAsync()
		{
			if (_videoPlayer == null)
			{
				Debug.LogWarning("[TitleManager] VideoPlayer が未設定です。", this);
				return;
			}

			_isPlaying = true;
			CacheAndHide();
			var gen = Interlocked.Increment(ref _playGeneration);

			try
			{
				if (_fadeCanvasGroup != null)
				{
					_fadeCanvasGroup.blocksRaycasts = true;
					_fadeCanvasGroup.interactable = true;
					await _fadeCanvasGroup.DOFade(1f, _fadeDuration).SetUpdate(true).AsyncWaitForCompletion();
					if (gen != _playGeneration) return;
				}

				_videoPlayer.Play();

				if (_fadeCanvasGroup != null)
				{
					await _fadeCanvasGroup.DOFade(0f, _fadeDuration).SetUpdate(true).AsyncWaitForCompletion();
					if (gen != _playGeneration) return;
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e, this);
				StopVideo();
			}
		}

		private void StopVideo()
		{
			Interlocked.Increment(ref _playGeneration);
			if (_videoPlayer != null)
			{
				_videoPlayer.Stop();
			}

			if (_fadeCanvasGroup != null)
			{
				_fadeCanvasGroup.DOKill();
				_fadeCanvasGroup.alpha = 0f;
				_fadeCanvasGroup.blocksRaycasts = false;
				_fadeCanvasGroup.interactable = false;
			}

			_isPlaying = false;
			_idleTimer = 0f;
			RestoreHidden();
		}

		private void CacheAndHide()
		{
			if (_hideOnPlay == null) return;

			_hideOnPlayCache = new bool[_hideOnPlay.Length];
			for (var i = 0; i < _hideOnPlay.Length; i++)
			{
				if (_hideOnPlay[i] != null)
				{
					_hideOnPlayCache[i] = _hideOnPlay[i].activeSelf;
					_hideOnPlay[i].SetActive(false);
				}
			}
		}

		private void RestoreHidden()
		{
			if (_hideOnPlay == null || _hideOnPlayCache == null) return;

			for (var i = 0; i < _hideOnPlay.Length; i++)
			{
				if (_hideOnPlay[i] != null)
				{
					_hideOnPlay[i].SetActive(_hideOnPlayCache[i]);
				}
			}
		}
	}
}
