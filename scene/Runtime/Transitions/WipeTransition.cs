using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameJamScene
{
	/// <summary>
	/// 横方向のワイプで画面を覆うトランジション。
	/// Canvas 配下に画面全体を覆うパネル（Stretch 設定）を作り、アタッチして使う。
	/// パネルの Anchors は四隅 Stretch（Min 0,0 / Max 1,1）、Offsets は全て 0 にしておくこと。
	/// </summary>
	public class WipeTransition : MonoBehaviour, ITransition
	{
		[SerializeField] private RectTransform _panel;
		[SerializeField] private float _duration = 0.4f;
		[SerializeField] private Ease _easeIn = Ease.OutQuad;
		[SerializeField] private Ease _easeOut = Ease.InQuad;

		private float _canvasWidth;

		private void Start()
		{
			var canvas = _panel.GetComponentInParent<Canvas>().rootCanvas;
			_canvasWidth = ((RectTransform)canvas.transform).rect.width;
			_panel.anchoredPosition = new Vector2(-_canvasWidth, 0f);
		}

		/// <summary>画面を左から右にワイプして覆う。</summary>
		public async UniTask Play()
		{
			await _panel.DOAnchorPosX(0f, _duration)
				.SetEase(_easeIn).SetUpdate(true).ToUniTask();
		}

		/// <summary>画面を左から右にワイプして開く。</summary>
		public async UniTask Release()
		{
			await _panel.DOAnchorPosX(_canvasWidth, _duration)
				.SetEase(_easeOut).SetUpdate(true).ToUniTask();

			_panel.anchoredPosition = new Vector2(-_canvasWidth, 0f);
		}
	}
}
