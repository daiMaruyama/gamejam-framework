using DG.Tweening;
using GameJamCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameJamAudio
{
	/// <summary>
	/// 音量スライダーの基底クラス。
	/// FillAmount 式の Image をアサインするだけで動作する。
	/// Canvas 配下の GameObject にアタッチすること。
	/// </summary>
	public abstract class VolumeSliderBase : MonoBehaviour, IPointerDownHandler, IDragHandler
	{
		/// <summary>fillAmount で音量を表示する Image。</summary>
		[SerializeField] private Image _fillImage;

		/// <summary>音量変化アニメーションの時間（秒）。</summary>
		[SerializeField] private float _animDuration = 0.1f;

		private RectTransform _fillRect;

		/// <summary>現在の音量を IAudioService から取得する。</summary>
		protected abstract float GetVolume(IAudioService audio);

		/// <summary>音量を IAudioService に設定する。</summary>
		protected abstract void SetVolume(IAudioService audio, float volume);

		private void Start()
		{
			if (_fillImage == null)
			{
				Debug.LogError($"[{GetType().Name}] Fill Image が未アサインです。", this);
				return;
			}

			_fillRect = _fillImage.rectTransform;

			if (ServiceLocator.TryGet<IAudioService>(out var audio))
			{
				SetFill(GetVolume(audio), animate: false);
			}
		}

		/// <summary>クリックした位置に音量を即時変更する。</summary>
		public void OnPointerDown(PointerEventData eventData)
		{
			ApplyVolume(eventData);
		}

		/// <summary>ドラッグ中に音量を更新する。</summary>
		public void OnDrag(PointerEventData eventData)
		{
			ApplyVolume(eventData);
		}

		private void ApplyVolume(PointerEventData eventData)
		{
			if (_fillRect == null)
			{
				return;
			}

			if (!ServiceLocator.TryGet<IAudioService>(out var audio))
			{
				return;
			}

			var volume = GetVolumeFromPointer(eventData);
			SetVolume(audio, volume);
			SetFill(volume, animate: true);
		}

		private float GetVolumeFromPointer(PointerEventData eventData)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				_fillRect, eventData.position, eventData.pressEventCamera, out var local);

			var rect = _fillRect.rect;
			return Mathf.Clamp01((local.x - rect.xMin) / rect.width);
		}

		private void SetFill(float value, bool animate)
		{
			_fillImage.DOKill();

			if (animate)
			{
				_fillImage.DOFillAmount(value, _animDuration).SetUpdate(true);
			}
			else
			{
				_fillImage.fillAmount = value;
			}
		}
	}
}
