using UnityEngine;

namespace GameJamAudio
{
	/// <summary>
	/// SE のデータを保持する ScriptableObject。
	/// Create > GameJam > Audio > SEData でアセットを作成する。
	/// </summary>
	[CreateAssetMenu(menuName = "GameJam/Audio/SEData", fileName = "SEData")]
	public class SEData : ScriptableObject
	{
		[SerializeField] private string _key;
		[SerializeField] private AudioClip _clip;

		/// <summary>0〜1 の音量。デフォルトは 1。</summary>
		[SerializeField, Range(0f, 1f)] private float _volume = 1f;

		/// <summary>SE を識別するキー文字列。</summary>
		public string Key => _key;

		/// <summary>再生する AudioClip。</summary>
		public AudioClip Clip => _clip;

		/// <summary>再生音量（0〜1）。</summary>
		public float Volume => _volume;
	}
}
