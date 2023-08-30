using UnityEngine;

[CreateAssetMenu(fileName = "NewSong", menuName = "Song Tracking/SongMeta", order = 1)]
public class SongMeta : ScriptableObject
{
	public AudioClip clip;
	[Min(0)]
	public float BPM = 120;
	[Min(0)]
	public float startOffset; // In seconds
}