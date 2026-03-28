using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameJamScene
{
	/// <summary>
	/// ルール画像シェーダーによる汎用トランジション。
	/// グレースケールのルール画像を差し替えるだけで、ワイプ・ディゾルブ・ダイヤ等
	/// あらゆるパターンの遷移演出を実現できる。
	/// Canvas 上に画面全体を覆う Image を持つ GameObject にアタッチして使う。
	/// </summary>
	public class ShaderTransition : TransitionBase
	{
		/// <summary>画面全体を覆う Image。Sprite は不要（シェーダーが描画を担う）。</summary>
		[SerializeField] private Image _overlay;

		/// <summary>
		/// 遷移パターンを決めるグレースケール画像。
		/// 暗いピクセルから先に覆われる。
		/// </summary>
		[SerializeField] private Texture2D _ruleTexture;

		[SerializeField] private Color _color = Color.black;
		[SerializeField] private float _duration = 0.5f;

		/// <summary>遷移の境界をぼかす量。0 でくっきり、0.5 で最大ぼかし。</summary>
		[SerializeField, Range(0f, 0.5f)] private float _softness = 0.05f;

		[SerializeField] private Ease _easeIn = Ease.OutQuad;
		[SerializeField] private Ease _easeOut = Ease.InQuad;

		private Material _material;
		private bool _isInitialized;

		private static readonly int ProgressId = Shader.PropertyToID("_Progress");
		private static readonly int SoftnessId = Shader.PropertyToID("_Softness");
		private static readonly int ColorId = Shader.PropertyToID("_Color");
		private static readonly int RuleTexId = Shader.PropertyToID("_RuleTex");

		private void Awake()
		{
			if (_overlay == null || _ruleTexture == null)
			{
				Debug.LogError("ShaderTransition requires Image and RuleTexture references.", this);
				enabled = false;
				return;
			}

			var shader = Shader.Find("GameJam/RuleTransition");

			if (shader == null)
			{
				Debug.LogError("Shader 'GameJam/RuleTransition' not found. Always Include に追加してください。", this);
				enabled = false;
				return;
			}

			_material = new Material(shader);
			_material.SetTexture(RuleTexId, _ruleTexture);
			_material.SetColor(ColorId, _color);
			_material.SetFloat(SoftnessId, _softness);
			_material.SetFloat(ProgressId, 0f);

			_overlay.material = _material;
			_overlay.raycastTarget = false;
			_isInitialized = true;
		}

		private void OnDestroy()
		{
			if (_material != null)
			{
				Destroy(_material);
			}
		}

		/// <summary>
		/// ルール画像のパターンに従って画面を覆う。
		/// </summary>
		public override async UniTask Play()
		{
			if (!_isInitialized)
			{
				return;
			}

			_overlay.raycastTarget = true;

			await DOTween.To(
				() => _material.GetFloat(ProgressId),
				x => _material.SetFloat(ProgressId, x),
				1f,
				_duration
			).SetEase(_easeIn).SetUpdate(true).ToUniTask();
		}

		/// <summary>
		/// ルール画像のパターンに従って画面を開く。
		/// </summary>
		public override async UniTask Release()
		{
			if (!_isInitialized)
			{
				return;
			}

			await DOTween.To(
				() => _material.GetFloat(ProgressId),
				x => _material.SetFloat(ProgressId, x),
				0f,
				_duration
			).SetEase(_easeOut).SetUpdate(true).ToUniTask();

			_overlay.raycastTarget = false;
		}
	}
}
