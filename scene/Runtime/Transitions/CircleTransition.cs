using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameJamScene
{
	/// <summary>
	/// 円形に閉じて画面を覆うトランジション。
	/// 空の GameObject にアタッチするだけで、子要素は自動生成される。
	/// 親の RectTransform を画面全体に引き伸ばして使う。
	/// </summary>
	public class CircleTransition : TransitionBase
	{
		[SerializeField] private Color _color = Color.black;
		[SerializeField] private float _duration = 0.5f;
		[SerializeField] private Ease _easeIn = Ease.OutQuad;
		[SerializeField] private Ease _easeOut = Ease.InQuad;

		private Image _circleImage;
		private bool _isInitialized;

		private void Awake()
		{
			var go = new GameObject("CircleOverlay");
			go.transform.SetParent(transform, false);

			_circleImage = go.AddComponent<Image>();
			_circleImage.color = _color;
			_circleImage.raycastTarget = false;
			_circleImage.type = Image.Type.Filled;
			_circleImage.fillMethod = Image.FillMethod.Radial360;
			_circleImage.fillOrigin = (int)Image.Origin360.Top;
			_circleImage.fillAmount = 0f;

			var rt = _circleImage.rectTransform;
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;

			_isInitialized = true;
		}

		/// <summary>
		/// 円形に閉じて画面を覆う。
		/// </summary>
		public override async UniTask Play()
		{
			if (!_isInitialized)
			{
				return;
			}

			_circleImage.raycastTarget = true;
			await _circleImage.DOFillAmount(1f, _duration)
				.SetEase(_easeIn).SetUpdate(true).ToUniTask();
		}

		/// <summary>
		/// 円形に開いて画面を見せる。
		/// </summary>
		public override async UniTask Release()
		{
			if (!_isInitialized)
			{
				return;
			}

			await _circleImage.DOFillAmount(0f, _duration)
				.SetEase(_easeOut).SetUpdate(true).ToUniTask();
			_circleImage.raycastTarget = false;
		}
	}
}
