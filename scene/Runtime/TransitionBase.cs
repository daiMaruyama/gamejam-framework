using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameJamScene
{
	/// <summary>
	/// 遷移コンポーネントの基底クラス。
	/// SceneLoader の参照を型安全にするために使う。
	/// </summary>
	public abstract class TransitionBase : MonoBehaviour, ITransition
	{
		public abstract UniTask Play();
		public abstract UniTask Release();
	}
}
