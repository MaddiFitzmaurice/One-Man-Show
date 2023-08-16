using UnityEngine;

[CreateAssetMenu(fileName = "NewSong", menuName = "Song Tracking/SongMeta", order = 1)]
public class SongMeta : ScriptableObject
{
	public string songName = "";
	public float BPM = 120;
	public float startOffset;
}