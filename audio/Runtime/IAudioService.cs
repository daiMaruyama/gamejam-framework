using Cysharp.Threading.Tasks;

namespace GameJamAudio
{
	/// <summary>
	/// BGM・SE 再生サービスのインターフェース。
	/// ServiceLocator.Get&lt;IAudioService&gt;() で取得して使う。
	/// </summary>
	public interface IAudioService
	{
		/// <summary>現在の BGM 音量（0〜1）。</summary>
		float BGMVolume { get; }

		/// <summary>現在の SE 音量（0〜1）。</summary>
		float SEVolume { get; }

		/// <summary>BGM を再生する。再生中の BGM はフェードアウトしてから切り替わる。</summary>
		UniTask PlayBGMAsync(BGMData data);

		/// <summary>BGM をフェードアウトして停止する。</summary>
		UniTask StopBGMAsync();

		/// <summary>SE を再生する。プールから AudioSource を借りて再生する。</summary>
		void PlaySE(SEData data);

		/// <summary>BGM の音量を設定して PlayerPrefs に保存する（0〜1）。</summary>
		void SetBGMVolume(float volume);

		/// <summary>SE の音量を設定して PlayerPrefs に保存する（0〜1）。</summary>
		void SetSEVolume(float volume);
	}
}
