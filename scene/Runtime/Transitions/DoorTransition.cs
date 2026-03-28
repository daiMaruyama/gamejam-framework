using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameJamScene
{
	/// <summary>
	/// 左右2枚のパネルがドアのように閉じて開くトランジション。
	/// CSS の cubic-bezier 風の滑らかな減速で高級感のある演出を実現する。
	/// 空の GameObject にアタッチするだけで、子要素は自動生成される。
	/// 親の RectTransform を画面全体に引き伸ばして使う。
	/// </summary>
	public class DoorTransition : TransitionBase
	{
		[SerializeField] private Color _color = Color.black;
		[SerializeField] private float _duration = 0.4f;
		[SerializeField] private Ease _easeIn = Ease.OutCubic;
		[SerializeField] private Ease _easeOut = Ease.InCubic;

		private Image _leftDoor;
		private Image _rightDoor;
		private Sequence _sequence;
		private bool _isInitialized;

		private void Awake()
		{
			_leftDoor = CreateDoor("Door_Left", 0f, 0.5f, new Vector2(0f, 0.5f));
			_rightDoor = CreateDoor("Door_Right", 0.5f, 1f, new Vector2(1f, 0.5f));
			_isInitialized = true;
		}

		/// <summary>
		/// ドアパネルを生成する。
		/// pivot でスケール方向を決める（左ドアは左端基点、右ドアは右端基点）。
		/// </summary>
		private Image CreateDoor(string name, float anchorXMin, float anchorXMax, Vector2 pivot)
		{
			var go = new GameObject(name);
			go.transform.SetParent(transform, false);

			var img = go.AddComponent<Image>();
			img.color = _color;
			img.raycastTarget = false;

			var rt = img.rectTransform;
			rt.anchorMin = new Vector2(anchorXMin, 0f);
			rt.anchorMax = new Vector2(anchorXMax, 1f);
			rt.pivot = pivot;
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;
			rt.localScale = new Vector3(0f, 1f, 1f);

			return img;
		}

		private void OnDestroy()
		{
			_sequence?.Kill();
		}

		/// <summary>
		/// 左右のドアが外側から閉じて画面を覆う。
		/// </summary>
		public override async UniTask Play()
		{
			if (!_isInitialized)
			{
				return;
			}

			_sequence?.Kill();

			_leftDoor.raycastTarget = true;
			_rightDoor.raycastTarget = true;

			_sequence = DOTween.Sequence().SetUpdate(true)
				.Join(_leftDoor.rectTransform.DOScaleX(1f, _duration).SetEase(_easeIn))
				.Join(_rightDoor.rectTransform.DOScaleX(1f, _duration).SetEase(_easeIn));

			await _sequence.ToUniTask();
		}

		/// <summary>
		/// 左右のドアが中央から開いて画面を見せる。
		/// </summary>
		public override async UniTask Release()
		{
			if (!_isInitialized)
			{
				return;
			}

			_sequence?.Kill();

			_sequence = DOTween.Sequence().SetUpdate(true)
				.Join(_leftDoor.rectTransform.DOScaleX(0f, _duration).SetEase(_easeOut))
				.Join(_rightDoor.rectTransform.DOScaleX(0f, _duration).SetEase(_easeOut));

			await _sequence.ToUniTask();

			_leftDoor.raycastTarget = false;
			_rightDoor.raycastTarget = false;
		}
	}
}
