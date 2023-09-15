using UnityEngine;

[System.Serializable]
public struct EnemySpawn
{
	StageDirection direction;
	public uint enemy_index;
}

[System.Serializable]
public struct TrackBeat
{
	public float beat;

	public EnemySpawn[] enemies; // Enemies to spawn
	public EventType [] events; // Events to broadcast
}

[System.Serializable]
[CreateAssetMenu(fileName = "NewTrack", menuName = "Song Tracking/TrackData", order = 2)]
public class TrackData : ScriptableObject
{
	public TrackBeat[] beats;
}