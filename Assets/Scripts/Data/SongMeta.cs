using UnityEngine;

[CreateAssetMenu(fileName = "NewSong", menuName = "Song Tracking/SongMeta", order = 1)]
public class SongMeta : ScriptableObject
{
	[SerializeField] public AudioClip clip;
	[Min(0)]
	[SerializeField] public double BPM = 120;
	[Min(0)]
	[SerializeField] public double startOffset; // In seconds
}