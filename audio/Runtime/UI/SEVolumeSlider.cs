using DG.Tweening;
using GameJamCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameJamAudio
{
	/// <summary>
	/// SE 音量を操作する FillAmount 式スライダー。
	/// Image をアサインするだけで動作する。PlayerPrefs に自動保存・ロードされる。
	/// Canvas 配下の GameObject にアタッチすること。
	/// </summary>
	public class SEVolumeSlider : MonoBehaviour, IPointerDownHandler, IDragHandler
	{
		/// <summary>fillAmount で音量を表示する Image。</summary>
		[SerializeField] private Image _fillImage;

		/// <summary>音量変化アニメーションの時間（秒）。</summary>
		[SerializeField] private float _animDuration = 0.1f;

		private RectTransform _fillRect;

		private void Start()
		{
			_fillRect = _fillImage.rectTransform;

			if (ServiceLocator.TryGet<IAudioService>(out var audio))
			{
				SetFill(audio.SEVolume, animate: false);
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
			if (!ServiceLocator.TryGet<IAudioService>(out var audio))
			{
				return;
			}

			var volume = GetVolumeFromPointer(eventData);
			audio.SetSEVolume(volume);
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
