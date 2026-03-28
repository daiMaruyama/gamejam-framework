using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameJamScene
{
	/// <summary>
	/// 遷移コンポーネントの基底クラス。
	/// 全トランジション共通の Inspector 項目（色・速度・イージング）を持つ。
	/// Canvas 配下の RectTransform を持つ GameObject にアタッチすること。
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public abstract class TransitionBase : MonoBehaviour, ITransition
	{
		[SerializeField] protected Color _color = Color.black;
		[SerializeField] protected float _duration = 0.4f;
		[SerializeField] protected Ease _easeIn = Ease.OutQuad;
		[SerializeField] protected Ease _easeOut = Ease.InQuad;

		protected bool _isInitialized;

		public abstract UniTask Play();
		public abstract UniTask Release();
	}
}
