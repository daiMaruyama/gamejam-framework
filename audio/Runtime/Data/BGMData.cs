using UnityEngine;

namespace GameJamAudio
{
	/// <summary>
	/// BGM のデータを保持する ScriptableObject。
	/// Create > GameJam > Audio > BGMData でアセットを作成する。
	/// </summary>
	[CreateAssetMenu(menuName = "GameJam/Audio/BGMData", fileName = "BGMData")]
	public class BGMData : ScriptableObject
	{
		[SerializeField] private AudioClip _clip;

		/// <summary>0〜1 の音量。デフォルトは 1。</summary>
		[SerializeField, Range(0f, 1f)] private float _volume = 1f;

		[SerializeField] private bool _loop = true;

		/// <summary>再生する AudioClip。</summary>
		public AudioClip Clip => _clip;

		/// <summary>再生音量（0〜1）。</summary>
		public float Volume => _volume;

		/// <summary>ループ再生するかどうか。</summary>
		public bool Loop => _loop;
	}
}
