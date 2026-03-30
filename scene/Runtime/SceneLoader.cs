using Cysharp.Threading.Tasks;
using GameJamCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJamScene
{
	/// <summary>
	/// シーン遷移サービス。
	/// Scene 上に配置して ServiceLocator に自動登録される。
	/// ServiceLocator.Get<ISceneService>().LoadAsync("Title") で呼び出せる。
	/// </summary>
	public class SceneLoader : MonoBehaviour, ISceneService
	{
		/// <summary>ITransition を実装した遷移コンポーネント。</summary>
		[SerializeField] private TransitionBase _defaultTransitionComponent;

		private TransitionBase _defaultTransition;
		private bool _isLoading;

		private static bool IsTransitionAlive(TransitionBase transition)
		{
			return transition != null;
		}

		private void Awake()
		{
			if (ServiceLocator.TryGet<ISceneService>(out _))
			{
				if (_defaultTransitionComponent != null && _defaultTransitionComponent.transform.root != transform.root)
				{
					Destroy(_defaultTransitionComponent.transform.root.gameObject);
				}
				Destroy(gameObject);
				return;
			}

			_defaultTransition = _defaultTransitionComponent;

			// Keep transition visuals alive across scene load so Release can run after LoadSceneAsync.
			if (_defaultTransitionComponent != null && _defaultTransitionComponent.transform.root != transform.root)
			{
				DontDestroyOnLoad(_defaultTransitionComponent.transform.root.gameObject);
			}

			DontDestroyOnLoad(gameObject);
			ServiceLocator.Register<ISceneService>(this);
		}

		/// <summary>
		/// 破棄時に ServiceLocator から自身を解除する。
		/// 通常は DontDestroyOnLoad のためアプリ終了時のみだが、
		/// テストやサービス差し替え時にゴミが残るのを防ぐ。
		/// </summary>
		private void OnDestroy()
		{
			if (ServiceLocator.TryGet<ISceneService>(out var registered) && registered == this)
			{
				ServiceLocator.Unregister<ISceneService>();
			}
		}

		/// <summary>
		/// トランジション付きでシーンを読み込む。
		/// transition を省略すると Inspector で設定したデフォルトトランジションを使う。
		/// 読み込み中に呼ばれた場合は無視される。
		/// </summary>
		public async UniTask LoadAsync(string sceneName, TransitionBase transition = null)
		{
			if (_isLoading)
			{
				return;
			}

			_isLoading = true;

			try
			{
				var activeTransition = transition ?? _defaultTransition;

				if (IsTransitionAlive(activeTransition))
				{
					await activeTransition.Play();
				}

				await SceneManager.LoadSceneAsync(sceneName);

				if (IsTransitionAlive(activeTransition))
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
