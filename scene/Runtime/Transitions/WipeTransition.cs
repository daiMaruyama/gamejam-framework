using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameJamScene
{
	/// <summary>
	/// 左右方向のワイプで画面を覆うトランジション。
	/// Canvas 上に画面全幅を覆う panel を用意し、アタッチして使う。
	/// Pivot の X を 0 にすると左から、1 にすると右からワイプする。
	/// </summary>
	public class WipeTransition : TransitionBase
	{
		[SerializeField] private RectTransform _panel;
		[SerializeField] private float _duration = 0.4f;
		[SerializeField] private Ease _easeIn = Ease.OutQuad;
		[SerializeField] private Ease _easeOut = Ease.InQuad;
		private bool _isInitialized;

		private void Awake()
		{
			if (_panel == null)
			{
				Debug.LogError("WipeTransition requires RectTransform panel reference.", this);
				enabled = false;
				return;
			}

			_panel.localScale = new Vector3(0f, 1f, 1f);
			_isInitialized = true;
		}

		public override async UniTask Play()
		{
			if (!_isInitialized) return;

			await _panel.DOScaleX(1f, _duration)
				.SetEase(_easeIn).SetUpdate(true).ToUniTask();
		}

		public override async UniTask Release()
		{
			if (!_isInitialized) return;

			await _panel.DOScaleX(0f, _duration)
				.SetEase(_easeOut).SetUpdate(true).ToUniTask();
		}
	}
}
