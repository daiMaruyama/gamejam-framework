namespace GameJamAudio
{
	/// <summary>
	/// SE 音量を操作する FillAmount 式スライダー。
	/// Canvas 配下の GameObject にアタッチして Fill Image をアサインするだけで動作する。
	/// </summary>
	public class SEVolumeSlider : VolumeSliderBase
	{
		protected override float GetVolume(IAudioService audio) => audio.SEVolume;

		protected override void SetVolume(IAudioService audio, float volume) => audio.SetSEVolume(volume);
	}
}
