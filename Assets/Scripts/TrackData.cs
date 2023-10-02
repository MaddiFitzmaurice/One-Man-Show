using System;
using UnityEngine;

[System.Serializable]
public struct SpawnBeat : IComparable<SpawnBeat>
{
	public float beat;
	public StageDirection direction;
	public EnemyType type;

	public int CompareTo(SpawnBeat other)
	{
		return beat.CompareTo(other.beat);
	}
}

[System.Serializable]
public struct EventBeat : IComparable<EventBeat>
{
	public float beat;
	public EventType type;
	public UnityEngine.Object data;

	public int CompareTo(EventBeat other)
	{
		return beat.CompareTo(other.beat);
	}
}

[System.Serializable]
[CreateAssetMenu(fileName = "NewTrack", menuName = "Song Tracking/TrackData", order = 2)]
public class TrackData : ScriptableObject
{
	public SongMeta song;
	public SpawnBeat[] enemies; // Enemies to spawn
	public EventBeat[] events; // Events to broadcast
}