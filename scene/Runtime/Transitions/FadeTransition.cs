using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameJamScene
{
	/// <summary>
	/// フェードで画面を覆うトランジション。
	/// CanvasGroup を持つ GameObject にアタッチして使う。
	/// </summary>
	public class FadeTransition : TransitionBase
	{
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private float _duration = 0.4f;
		[SerializeField] private Ease _easeIn = Ease.OutQuad;
		[SerializeField] private Ease _easeOut = Ease.InQuad;
		private bool _isInitialized;

		private void Awake()
		{
			if (_canvasGroup == null)
			{
				Debug.LogError("FadeTransition requires CanvasGroup reference.", this);
				enabled = false;
				return;
			}

			_canvasGroup.alpha = 0f;
			_canvasGroup.blocksRaycasts = false;
			_isInitialized = true;
		}

		public override async UniTask Play()
		{
			if (!_isInitialized) return;

			_canvasGroup.blocksRaycasts = true;
			await _canvasGroup.DOFade(1f, _duration).SetEase(_easeIn)
				.SetUpdate(true).ToUniTask();
		}

		public override async UniTask Release()
		{
			if (!_isInitialized) return;

			await _canvasGroup.DOFade(0f, _duration).SetEase(_easeOut)
				.SetUpdate(true).ToUniTask();
			_canvasGroup.blocksRaycasts = false;
		}
	}
}
