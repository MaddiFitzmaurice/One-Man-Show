using System;
using UnityEngine;

[System.Serializable]
public struct EnemySpawn
{
	public StageDirection direction;
	public EnemyType type;
}

[System.Serializable]
public struct SpawnEvent
{
	public EventType type;
	public UnityEngine.Object data;
}

[System.Serializable]
public struct TrackBeat : IComparable<TrackBeat>
{
	public float beat;

	public EnemySpawn[] enemies; // Enemies to spawn
	public SpawnEvent[] events; // Events to broadcast

	public int CompareTo(TrackBeat other)
	{
		return beat.CompareTo(other.beat);
	}
}

[System.Serializable]
[CreateAssetMenu(fileName = "NewTrack", menuName = "Song Tracking/TrackData", order = 2)]
public class TrackData : ScriptableObject
{
	public TrackBeat[] beats;
	public SongMeta song;
}