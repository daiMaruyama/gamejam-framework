using Cysharp.Threading.Tasks;

namespace GameJamScene
{
	/// <summary>
	/// シーン遷移時の演出インターフェース。
	/// Play() で画面を覆い、Release() で画面を開く。
	/// </summary>
	public interface ITransition
	{
		UniTask Play();
		UniTask Release();
	}
}
