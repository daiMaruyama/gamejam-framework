using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameJamScene
{
	/// <summary>
	/// フェードで画面を覆うトランジション。
	/// 空の GameObject にアタッチするだけで、子要素は自動生成される。
	/// 親の RectTransform を画面全体に引き伸ばして使う。
	/// </summary>
	public class FadeTransition : TransitionBase
	{
		private Image _overlay;

		private void Awake()
		{
			var go = new GameObject("FadeOverlay");
			go.transform.SetParent(transform, false);

			_overlay = go.AddComponent<Image>();
			_overlay.color = new Color(_color.r, _color.g, _color.b, 0f);
			_overlay.raycastTarget = false;

			var rt = _overlay.rectTransform;
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;

			_isInitialized = true;
		}

		/// <summary>
		/// 画面をフェードインで覆う。
		/// </summary>
		public override async UniTask Play()
		{
			if (!_isInitialized)
			{
				return;
			}

			_overlay.raycastTarget = true;
			await _overlay.DOFade(1f, _duration).SetEase(_easeIn)
				.SetUpdate(true).ToUniTask();
		}

		/// <summary>
		/// 画面をフェードアウトで開く。
		/// </summary>
		public override async UniTask Release()
		{
			if (!_isInitialized)
			{
				return;
			}

			await _overlay.DOFade(0f, _duration).SetEase(_easeOut)
				.SetUpdate(true).ToUniTask();
			_overlay.raycastTarget = false;
		}
	}
}
