using Cysharp.Threading.Tasks;
using GameJamCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJamScene
{
	/// <summary>
	/// シーン遷移サービス。
	/// シーンに置くだけで ServiceLocator に自己登録される。
	/// ServiceLocator.Get&lt;ISceneService&gt;().LoadAsync("Title") で遷移できる。
	/// </summary>
	public class SceneLoader : MonoBehaviour, ISceneService
	{
		/// <summary>ITransition を実装した MonoBehaviour をアタッチする。</summary>
		[SerializeField] private MonoBehaviour _defaultTransitionComponent;

		private ITransition _defaultTransition;

		private void Awake()
		{
			_defaultTransition = _defaultTransitionComponent as ITransition;
			DontDestroyOnLoad(gameObject);
			ServiceLocator.Register<ISceneService>(this);
		}

		/// <summary>
		/// トランジション付きでシーンを遷移する。
		/// transition を省略すると Inspector で設定したデフォルトトランジションを使う。
		/// </summary>
		public async UniTask LoadAsync(string sceneName, ITransition transition = null)
		{
			var activeTransition = transition ?? _defaultTransition;

			if (activeTransition != null)
			{
				await activeTransition.Play();
			}

			await SceneManager.LoadSceneAsync(sceneName);

			if (activeTransition != null)
			{
				await activeTransition.Release();
			}
		}
	}
}
