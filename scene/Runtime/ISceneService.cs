using Cysharp.Threading.Tasks;

namespace GameJamScene
{
	/// <summary>
	/// シーン遷移サービスのインターフェース。
	/// ServiceLocator.Get&lt;ISceneService&gt;() で取得して使う。
	/// </summary>
	public interface ISceneService
	{
		UniTask LoadAsync(string sceneName, TransitionBase transition = null);
	}
}
