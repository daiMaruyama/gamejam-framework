using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameJamScene
{
	/// <summary>
	/// 円形に閉じて画面を覆うトランジション。
	/// Unity 標準の Image(Filled, Radial 360) を使う。
	/// Canvas 上に Filled の円形 Image を持つ GameObject にアタッチして使う。
	/// </summary>
	public class CircleTransition : TransitionBase
	{
		/// <summary>Image の Type は Filled、Fill Method は Radial 360 に設定しておくこと。</summary>
		[SerializeField] private Image _circleImage;
		[SerializeField] private float _duration = 0.5f;
		[SerializeField] private Ease _easeIn = Ease.OutQuad;
		[SerializeField] private Ease _easeOut = Ease.InQuad;
		private bool _isInitialized;

		private void Awake()
		{
			if (_circleImage == null)
			{
				Debug.LogError("CircleTransition requires Image reference.", this);
				enabled = false;
				return;
			}

			_circleImage.fillAmount = 0f;
			_circleImage.raycastTarget = false;
			_isInitialized = true;
		}

		public override async UniTask Play()
		{
			if (!_isInitialized) return;

			_circleImage.raycastTarget = true;
			await _circleImage.DOFillAmount(1f, _duration)
				.SetEase(_easeIn).SetUpdate(true).ToUniTask();
		}

		public override async UniTask Release()
		{
			if (!_isInitialized) return;

			await _circleImage.DOFillAmount(0f, _duration)
				.SetEase(_easeOut).SetUpdate(true).ToUniTask();
			_circleImage.raycastTarget = false;
		}
	}
}
