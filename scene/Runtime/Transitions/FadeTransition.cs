using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameJamScene
{
	/// <summary>
	/// フェードで画面を覆うトランジション。
	/// CanvasGroup を持つ GameObject にアタッチして使う。
	/// </summary>
	public class FadeTransition : MonoBehaviour, ITransition
	{
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private float _duration = 0.4f;
		[SerializeField] private Ease _easeIn = Ease.OutQuad;
		[SerializeField] private Ease _easeOut = Ease.InQuad;

		private void Awake()
		{
			_canvasGroup.alpha = 0f;
			_canvasGroup.blocksRaycasts = false;
		}

		/// <summary>画面を覆う（フェードイン）。</summary>
		public async UniTask Play()
		{
			_canvasGroup.blocksRaycasts = true;
			await _canvasGroup.DOFade(1f, _duration).SetEase(_easeIn)
				.SetUpdate(true).AsyncWaitForCompletion();
		}

		/// <summary>画面を開く（フェードアウト）。</summary>
		public async UniTask Release()
		{
			await _canvasGroup.DOFade(0f, _duration).SetEase(_easeOut)
				.SetUpdate(true).AsyncWaitForCompletion();
			_canvasGroup.blocksRaycasts = false;
		}
	}
}
