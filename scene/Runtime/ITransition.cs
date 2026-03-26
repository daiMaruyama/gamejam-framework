using Cysharp.Threading.Tasks;

namespace GameJamScene
{
    public interface ITransition
    {
        UniTask Play();
        UniTask Release();
    }
}
