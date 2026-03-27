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
		private bool _isLoading;

		private void Awake()
		{
			if (ServiceLocator.TryGet<ISceneService>(out _))
			{
				Destroy(gameObject);
				return;
			}

			_defaultTransition = _defaultTransitionComponent as ITransition;
			DontDestroyOnLoad(gameObject);
			ServiceLocator.Register<ISceneService>(this);
		}

		/// <summary>
		/// 破棄時に ServiceLocator から自身を解除する。
		/// 通常は DontDestroyOnLoad のためアプリ終了時のみ呼ばれるが、
		/// テストやサービス差し替え時に電話帳にゴミが残るのを防ぐ。
		/// </summary>
		private void OnDestroy()
		{
			if (ServiceLocator.TryGet<ISceneService>(out var registered) && registered == this)
			{
				ServiceLocator.Unregister<ISceneService>();
			}
		}

		/// <summary>
		/// トランジション付きでシーンを遷移する。
		/// transition を省略すると Inspector で設定したデフォルトトランジションを使う。
		/// 遷移中に呼ばれた場合は無視される。
		/// </summary>
		public async UniTask LoadAsync(string sceneName, ITransition transition = null)
		{
			if (_isLoading)
			{
				return;
			}

			_isLoading = true;

			try
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
			finally
			{
				_isLoading = false;
			}
		}
	}
}
