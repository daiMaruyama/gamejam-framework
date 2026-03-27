using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameJamScene
{
	/// <summary>
	/// 円形に閉じて画面を覆うトランジション。
	/// SpriteMask や円形 Image を使わず、Unity 標準の Image（Filled）で実現する。
	/// Canvas 配下に Filled の円形 Image を持つ GameObject にアタッチして使う。
	/// </summary>
	public class CircleTransition : MonoBehaviour, ITransition
	{
		/// <summary>Image の Type を Filled、Fill Method を Radial 360 に設定しておくこと。</summary>
		[SerializeField] private Image _circleImage;
		[SerializeField] private float _duration = 0.5f;
		[SerializeField] private Ease _easeIn = Ease.OutQuad;
		[SerializeField] private Ease _easeOut = Ease.InQuad;

		private void Awake()
		{
			_circleImage.fillAmount = 0f;
			_circleImage.raycastTarget = false;
		}

		/// <summary>円を広げて画面を覆う。</summary>
		public async UniTask Play()
		{
			_circleImage.raycastTarget = true;
			await _circleImage.DOFillAmount(1f, _duration)
				.SetEase(_easeIn).SetUpdate(true).ToUniTask();
		}

		/// <summary>円を縮めて画面を開く。</summary>
		public async UniTask Release()
		{
			await _circleImage.DOFillAmount(0f, _duration)
				.SetEase(_easeOut).SetUpdate(true).ToUniTask();
			_circleImage.raycastTarget = false;
		}
	}
}
