namespace GameJamAudio
{
	/// <summary>
	/// BGM 音量を操作する FillAmount 式スライダー。
	/// Canvas 配下の GameObject にアタッチして Fill Image をアサインするだけで動作する。
	/// </summary>
	public class BGMVolumeSlider : VolumeSliderBase
	{
		protected override float GetVolume(IAudioService audio) => audio.BGMVolume;

		protected override void SetVolume(IAudioService audio, float volume) => audio.SetBGMVolume(volume);
	}
}
