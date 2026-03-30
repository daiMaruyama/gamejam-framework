using Cysharp.Threading.Tasks;

namespace GameJamAudio
{
	/// <summary>
	/// BGM・SE 再生サービスのインターフェース。
	/// ServiceLocator.Get&lt;IAudioService&gt;() で取得して使う。
	/// </summary>
	public interface IAudioService
	{
		/// <summary>BGM を再生する。再生中の BGM はフェードアウトしてから切り替わる。</summary>
		UniTask PlayBGMAsync(BGMData data);

		/// <summary>BGM をフェードアウトして停止する。</summary>
		UniTask StopBGMAsync();

		/// <summary>SE を再生する。プールから AudioSource を借りて再生する。</summary>
		void PlaySE(SEData data);
	}
}
