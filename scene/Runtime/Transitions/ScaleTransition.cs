using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameJamScene
{
	/// <summary>
	/// アタッチした GameObject をスケールアニメーションで拡縮するトランジション。
	/// Play() でゼロに縮み、Release() で元のスケールに戻る。
	/// 空の GameObject にアタッチするだけで動作する。
	/// </summary>
	public class ScaleTransition : TransitionBase
	{
		private Vector3 _defaultScale;
		private Image _blocker;

		private void Awake()
		{
			_defaultScale = transform.localScale;

			_blocker = gameObject.AddComponent<Image>();
			_blocker.color = Color.clear;
			_blocker.raycastTarget = false;

			_isInitialized = true;
		}

		/// <summary>
		/// GameObject をスケールダウンして画面を切り替える準備をする。
		/// </summary>
		public override async UniTask Play()
		{
			if (!_isInitialized)
			{
				return;
			}

			_blocker.raycastTarget = true;
			await transform.DOScale(Vector3.zero, _duration)
				.SetEase(_easeIn).SetUpdate(true).ToUniTask();
		}

		/// <summary>
		/// GameObject を元のスケールに戻す。
		/// </summary>
		public override async UniTask Release()
		{
			if (!_isInitialized)
			{
				return;
			}

			await transform.DOScale(_defaultScale, _duration)
				.SetEase(_easeOut).SetUpdate(true).ToUniTask();
			_blocker.raycastTarget = false;
		}
	}
}
