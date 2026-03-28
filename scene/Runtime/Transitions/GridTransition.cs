using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameJamScene
{
	/// <summary>
	/// 格子状のタイルが中心から外側へ時間差でポップインするトランジション。
	/// CSS の radial stagger animation のようなモザイク演出を実現する。
	/// 空の GameObject にアタッチするだけで、子要素は自動生成される。
	/// 親の RectTransform を画面全体に引き伸ばして使う。
	/// </summary>
	public class GridTransition : TransitionBase
	{
		[SerializeField] private int _columns = 5;
		[SerializeField] private int _rows = 5;
		[SerializeField] private Color _color = Color.black;
		[SerializeField] private float _duration = 0.25f;

		/// <summary>中心から最も遠いタイルまでの最大遅延秒数。</summary>
		[SerializeField] private float _stagger = 0.3f;

		[SerializeField] private Ease _easeIn = Ease.OutBack;
		[SerializeField] private Ease _easeOut = Ease.InQuad;

		private Image _blocker;
		private Image[] _tiles;
		private float[] _distances;
		private float _maxDistance;
		private Sequence _sequence;
		private bool _isInitialized;

		private void Awake()
		{
			if (_columns <= 0 || _rows <= 0)
			{
				Debug.LogError("GridTransition requires columns > 0 and rows > 0.", this);
				enabled = false;
				return;
			}

			_blocker = gameObject.AddComponent<Image>();
			_blocker.color = Color.clear;
			_blocker.raycastTarget = false;

			int count = _columns * _rows;
			_tiles = new Image[count];
			_distances = new float[count];

			float centerX = (_columns - 1) / 2f;
			float centerY = (_rows - 1) / 2f;
			_maxDistance = Vector2.Distance(Vector2.zero, new Vector2(centerX, centerY));

			if (_maxDistance < 0.001f)
			{
				_maxDistance = 1f;
			}

			for (int y = 0; y < _rows; y++)
			{
				for (int x = 0; x < _columns; x++)
				{
					int index = y * _columns + x;

					var go = new GameObject($"Tile_{x}_{y}");
					go.transform.SetParent(transform, false);

					var img = go.AddComponent<Image>();
					img.color = _color;
					img.raycastTarget = false;

					var rt = img.rectTransform;
					rt.anchorMin = new Vector2((float)x / _columns, (float)y / _rows);
					rt.anchorMax = new Vector2((float)(x + 1) / _columns, (float)(y + 1) / _rows);
					rt.offsetMin = Vector2.zero;
					rt.offsetMax = Vector2.zero;
					rt.localScale = Vector3.zero;

					_tiles[index] = img;
					_distances[index] = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
				}
			}

			_isInitialized = true;
		}

		private void OnDestroy()
		{
			_sequence?.Kill();
		}

		/// <summary>
		/// タイルを中心から外側へ時間差でポップインさせて画面を覆う。
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

			for (int i = 0; i < _tiles.Length; i++)
			{
				float delay = (_distances[i] / _maxDistance) * _stagger;
				_sequence.Insert(delay, _tiles[i].rectTransform.DOScale(1f, _duration).SetEase(_easeIn));
			}

			await _sequence.ToUniTask();
		}

		/// <summary>
		/// タイルを外側から中心へ時間差で消して画面を開く。
		/// </summary>
		public override async UniTask Release()
		{
			if (!_isInitialized)
			{
				return;
			}

			_sequence?.Kill();
			_sequence = DOTween.Sequence().SetUpdate(true);

			for (int i = 0; i < _tiles.Length; i++)
			{
				float delay = (1f - _distances[i] / _maxDistance) * _stagger;
				_sequence.Insert(delay, _tiles[i].rectTransform.DOScale(0f, _duration).SetEase(_easeOut));
			}

			await _sequence.ToUniTask();

			_blocker.raycastTarget = false;
		}
	}
}
