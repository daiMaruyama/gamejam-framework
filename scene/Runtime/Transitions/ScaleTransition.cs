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
		private Transform _scaleTarget;
		private Image _blocker;

		private void Awake()
		{
			// ブロッカーは親（このオブジェクト）に置く。スケールの影響を受けない。
			_blocker = gameObject.AddComponent<Image>();
			_blocker.color = Color.clear;
			_blocker.raycastTarget = false;

			// スケールさせる子を作る。
			var go = new GameObject("ScaleTarget");
			go.transform.SetParent(transform, false);

			var overlay = go.AddComponent<Image>();
			overlay.color = _color;
			overlay.raycastTarget = false;

			var rt = overlay.rectTransform;
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;

			_scaleTarget = go.transform;
			_scaleTarget.localScale = Vector3.zero;

			_isInitialized = true;
		}

		/// <summary>
		/// 子オブジェクトをスケールアップして画面を覆う。
		/// </summary>
		public override async UniTask Play()
		{
			if (!_isInitialized)
			{
				return;
			}

			_blocker.raycastTarget = true;
			await _scaleTarget.DOScale(Vector3.one, _duration)
				.SetEase(_easeIn).SetUpdate(true).ToUniTask();
		}

		/// <summary>
		/// 子オブジェクトをスケールダウンして画面を開く。
		/// </summary>
		public override async UniTask Release()
		{
			if (!_isInitialized)
			{
				return;
			}

			await _scaleTarget.DOScale(Vector3.zero, _duration)
				.SetEase(_easeOut).SetUpdate(true).ToUniTask();
			_blocker.raycastTarget = false;
		}
	}
}
