using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameJamScene
{
	/// <summary>
	/// 遷移コンポーネントの基底クラス。
	/// SceneLoader の参照を型安全にするために使う。
	/// Canvas 配下の RectTransform を持つ GameObject にアタッチすること。
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public abstract class TransitionBase : MonoBehaviour, ITransition
	{
		public abstract UniTask Play();
		public abstract UniTask Release();
	}
}
