using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameJamScene
{
	/// <summary>
	/// 横方向のワイプで画面を覆うトランジション。
	/// 画面全体を覆える RectTransform を持つ UI にアタッチして使う。
	/// </summary>
	public class WipeTransition : MonoBehaviour, ITransition
	{
		[SerializeField] private RectTransform _panel;
		[SerializeField] private float _duration = 0.4f;
		[SerializeField] private Ease _easeIn = Ease.OutQuad;
		[SerializeField] private Ease _easeOut = Ease.InQuad;

		private void Awake()
		{
			_panel.anchorMin = new Vector2(-1f, 0f);
			_panel.anchorMax = new Vector2(0f, 1f);
			_panel.offsetMin = Vector2.zero;
			_panel.offsetMax = Vector2.zero;
		}

		/// <summary>画面を左から右にワイプして覆う。</summary>
		public async UniTask Play()
		{
			await DOTween.To(
				() => _panel.anchorMin.x,
				x =>
				{
					_panel.anchorMin = new Vector2(x, 0f);
					_panel.anchorMax = new Vector2(x + 1f, 1f);
				},
				0f,
				_duration
			).SetEase(_easeIn).SetUpdate(true).ToUniTask();
		}

		/// <summary>画面を左から右にワイプして開く。</summary>
		public async UniTask Release()
		{
			await DOTween.To(
				() => _panel.anchorMin.x,
				x =>
				{
					_panel.anchorMin = new Vector2(x, 0f);
					_panel.anchorMax = new Vector2(x + 1f, 1f);
				},
				1f,
				_duration
			).SetEase(_easeOut).SetUpdate(true).ToUniTask();

			_panel.anchorMin = new Vector2(-1f, 0f);
			_panel.anchorMax = new Vector2(0f, 1f);
		}
	}
}
