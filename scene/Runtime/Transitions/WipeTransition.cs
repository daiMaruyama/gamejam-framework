using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameJamScene
{
	/// <summary>
	/// 左右方向のワイプで画面を覆うトランジション。
	/// 空の GameObject にアタッチするだけで、子要素は自動生成される。
	/// 親の RectTransform を画面全体に引き伸ばして使う。
	/// </summary>
	public class WipeTransition : TransitionBase
	{
		private Image _panel;

		private void Awake()
		{
			var go = new GameObject("WipePanel");
			go.transform.SetParent(transform, false);

			_panel = go.AddComponent<Image>();
			_panel.color = _color;
			_panel.raycastTarget = false;

			var rt = _panel.rectTransform;
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.pivot = new Vector2(0f, 0.5f);
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;
			rt.localScale = new Vector3(0f, 1f, 1f);

			_isInitialized = true;
		}

		/// <summary>
		/// 左からワイプして画面を覆う。
		/// </summary>
		public override async UniTask Play()
		{
			if (!_isInitialized)
			{
				return;
			}

			_panel.raycastTarget = true;
			await _panel.rectTransform.DOScaleX(1f, _duration)
				.SetEase(_easeIn).SetUpdate(true).ToUniTask();
		}

		/// <summary>
		/// ワイプを戻して画面を開く。
		/// </summary>
		public override async UniTask Release()
		{
			if (!_isInitialized)
			{
				return;
			}

			await _panel.rectTransform.DOScaleX(0f, _duration)
				.SetEase(_easeOut).SetUpdate(true).ToUniTask();
			_panel.raycastTarget = false;
		}
	}
}
