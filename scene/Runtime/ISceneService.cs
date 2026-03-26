using Cysharp.Threading.Tasks;

namespace GameJamScene
{
    public interface ISceneService
    {
        UniTask LoadAsync(string sceneName, ITransition transition = null);
    }
}
