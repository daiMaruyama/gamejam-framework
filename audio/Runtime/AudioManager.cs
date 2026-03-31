using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameJamCore;
using UnityEngine;

namespace GameJamAudio
{
	/// <summary>
	/// BGM・SE 再生サービス。
	/// Scene 上に配置して ServiceLocator に自動登録される。
	/// ServiceLocator.Get&lt;IAudioService&gt;().PlayBGMAsync(data) で呼び出せる。
	/// </summary>
	public class AudioManager : MonoBehaviour, IAudioService
	{
		private const string BgmVolumeKey = "BGMVolume";
		private const string SeVolumeKey = "SEVolume";

		/// <summary>BGM のフェード時間（秒）。</summary>
		[SerializeField] private float _bgmFadeDuration = 0.5f;

		/// <summary>SE 再生に使う AudioSource のプール数。</summary>
		[SerializeField, Min(1)] private int _sePoolSize = 8;

		private AudioSource _bgmSource;
		private AudioSource[] _sePool;
		private int _sePoolIndex;
		private Tweener _bgmTween;
		private float _bgmVolume;
		private float _seVolume;

		public float BGMVolume => _bgmVolume;
		public float SEVolume => _seVolume;

		private void Awake()
		{
			if (ServiceLocator.TryGet<IAudioService>(out _))
			{
				Destroy(gameObject);
				return;
			}

			if (_sePoolSize <= 0)
			{
				Debug.LogError($"[{GetType().Name}] sePoolSize は 1 以上にしてください。", this);
				enabled = false;
				return;
			}

			_bgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, 1f);
			_seVolume = PlayerPrefs.GetFloat(SeVolumeKey, 1f);

			_bgmSource = gameObject.AddComponent<AudioSource>();
			_bgmSource.loop = false;
			_bgmSource.playOnAwake = false;
			_bgmSource.volume = _bgmVolume;

			_sePool = new AudioSource[_sePoolSize];

			for (int i = 0; i < _sePoolSize; i++)
			{
				_sePool[i] = gameObject.AddComponent<AudioSource>();
				_sePool[i].playOnAwake = false;
			}

			DontDestroyOnLoad(gameObject);
			ServiceLocator.Register<IAudioService>(this);
		}

		private void OnDestroy()
		{
			_bgmTween?.Kill();

			if (ServiceLocator.TryGet<IAudioService>(out var registered) && registered == this)
			{
				ServiceLocator.Unregister<IAudioService>();
			}
		}

		/// <summary>
		/// BGM を再生する。
		/// 再生中の BGM はフェードアウトしてから停止し、新しい BGM をフェードインする。
		/// </summary>
		public async UniTask PlayBGMAsync(BGMData data)
		{
			if (data == null || data.Clip == null)
			{
				Debug.LogWarning("[AudioManager] BGMData または Clip が null です。");
				return;
			}

			if (_bgmSource.isPlaying)
			{
				await FadeBGMAsync(0f);
				_bgmSource.Stop();
			}

			_bgmSource.clip = data.Clip;
			_bgmSource.loop = data.Loop;
			_bgmSource.volume = 0f;
			_bgmSource.Play();

			await FadeBGMAsync(data.Volume * _bgmVolume);
		}

		/// <summary>
		/// BGM をフェードアウトして停止する。
		/// </summary>
		public async UniTask StopBGMAsync()
		{
			if (!_bgmSource.isPlaying)
			{
				return;
			}

			await FadeBGMAsync(0f);
			_bgmSource.Stop();
		}

		/// <summary>
		/// SE を再生する。プールから空き AudioSource を借りて PlayOneShot する。
		/// </summary>
		public void PlaySE(SEData data)
		{
			if (data == null || data.Clip == null)
			{
				Debug.LogWarning("[AudioManager] SEData または Clip が null です。");
				return;
			}

			var source = _sePool[_sePoolIndex];
			_sePoolIndex = (_sePoolIndex + 1) % _sePoolSize;
			source.PlayOneShot(data.Clip, data.Volume * _seVolume);
		}

		/// <summary>
		/// BGM の音量を設定して PlayerPrefs に保存する。
		/// </summary>
		public void SetBGMVolume(float volume)
		{
			_bgmVolume = Mathf.Clamp01(volume);
			_bgmSource.volume = _bgmVolume;
			PlayerPrefs.SetFloat(BgmVolumeKey, _bgmVolume);
		}

		/// <summary>
		/// SE の音量を設定して PlayerPrefs に保存する。
		/// </summary>
		public void SetSEVolume(float volume)
		{
			_seVolume = Mathf.Clamp01(volume);
			PlayerPrefs.SetFloat(SeVolumeKey, _seVolume);
		}

		private UniTask FadeBGMAsync(float targetVolume)
		{
			_bgmTween?.Kill();

			if (_bgmFadeDuration <= 0f)
			{
				_bgmSource.volume = targetVolume;
				return UniTask.CompletedTask;
			}

			var tcs = new UniTaskCompletionSource();
			_bgmTween = DOTween.To(() => _bgmSource.volume, v => _bgmSource.volume = v, targetVolume, _bgmFadeDuration)
				.SetUpdate(true)
				.OnComplete(() => tcs.TrySetResult())
				.OnKill(() => tcs.TrySetResult());

			return tcs.Task;
		}
	}
}
