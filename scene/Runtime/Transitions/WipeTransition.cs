using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameJamScene
{
	/// <summary>
	/// 横方向のワイプで画面を覆うトランジション。
	/// Canvas 配下に画面全体を覆うパネル（Stretch 設定）を作り、アタッチして使う。
	/// Pivot の X を 0 にすると左から、1 にすると右からワイプする。
	/// </summary>
	public class WipeTransition : MonoBehaviour, ITransition
	{
		[SerializeField] private RectTransform _panel;
		[SerializeField] private float _duration = 0.4f;
		[SerializeField] private Ease _easeIn = Ease.OutQuad;
		[SerializeField] private Ease _easeOut = Ease.InQuad;

		private void Awake()
		{
			_panel.localScale = new Vector3(0f, 1f, 1f);
		}

		/// <summary>画面をワイプして覆う。</summary>
		public async UniTask Play()
		{
			await _panel.DOScaleX(1f, _duration)
				.SetEase(_easeIn).SetUpdate(true).ToUniTask();
		}

		/// <summary>画面をワイプして開く。</summary>
		public async UniTask Release()
		{
			await _panel.DOScaleX(0f, _duration)
				.SetEase(_easeOut).SetUpdate(true).ToUniTask();
		}
	}
}
