using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameJamScene
{
	/// <summary>
	/// 短冊状のストライプが時間差でスケールインする豪華なトランジション。
	/// CSS の staggered animation のような波状の演出を実現する。
	/// 空の GameObject にアタッチするだけで、子要素は自動生成される。
	/// 親の RectTransform を画面全体に引き伸ばして使う。
	/// </summary>
	public class StripeTransition : TransitionBase
	{
		[SerializeField] private int _stripeCount = 8;
		[SerializeField] private Color _color = Color.black;
		[SerializeField] private float _duration = 0.3f;

		/// <summary>各ストライプ間の遅延秒数。大きいほど波が目立つ。</summary>
		[SerializeField] private float _stagger = 0.05f;

		[SerializeField] private Ease _easeIn = Ease.OutBack;
		[SerializeField] private Ease _easeOut = Ease.InQuad;

		private Image _blocker;
		private Image[] _strips;
		private Sequence _sequence;
		private bool _isInitialized;

		private void Awake()
		{
			if (_stripeCount <= 0)
			{
				Debug.LogError("StripeTransition requires stripeCount > 0.", this);
				enabled = false;
				return;
			}

			_blocker = gameObject.AddComponent<Image>();
			_blocker.color = Color.clear;
			_blocker.raycastTarget = false;

			_strips = new Image[_stripeCount];

			for (int i = 0; i < _stripeCount; i++)
			{
				var go = new GameObject($"Stripe_{i}");
				go.transform.SetParent(transform, false);

				var img = go.AddComponent<Image>();
				img.color = _color;
				img.raycastTarget = false;

				var rt = img.rectTransform;
				float min = (float)i / _stripeCount;
				float max = (float)(i + 1) / _stripeCount;
				rt.anchorMin = new Vector2(min, 0f);
				rt.anchorMax = new Vector2(max, 1f);
				rt.offsetMin = Vector2.zero;
				rt.offsetMax = Vector2.zero;
				rt.localScale = new Vector3(1f, 0f, 1f);

				_strips[i] = img;
			}

			_isInitialized = true;
		}

		private void OnDestroy()
		{
			_sequence?.Kill();
		}

		/// <summary>
		/// ストライプを左から順に時間差でスケールインさせて画面を覆う。
		/// </summary>
		public override async UniTask Play()
		{
			if (!_isInitialized)
			{
				return;
			}

			_blocker.raycastTarget = true;

			_sequence?.Kill();
			_sequence = DOTween.Sequence().SetUpdate(true);

			for (int i = 0; i < _stripeCount; i++)
			{
				float delay = i * _stagger;
				_sequence.Insert(delay, _strips[i].rectTransform.DOScaleY(1f, _duration).SetEase(_easeIn));
			}

			await _sequence.ToUniTask();
		}

		/// <summary>
		/// ストライプを左から順に時間差でスケールアウトさせて画面を開く。
		/// </summary>
		public override async UniTask Release()
		{
			if (!_isInitialized)
			{
				return;
			}

			_sequence?.Kill();
			_sequence = DOTween.Sequence().SetUpdate(true);

			for (int i = 0; i < _stripeCount; i++)
			{
				float delay = i * _stagger;
				_sequence.Insert(delay, _strips[i].rectTransform.DOScaleY(0f, _duration).SetEase(_easeOut));
			}

			await _sequence.ToUniTask();

			_blocker.raycastTarget = false;
		}
	}
}
