using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
	//private static TrackManager instance = null;

	private TrackData _cur_track = null;
	private SongMeta _cur_song = null;
	
	private uint _beats_processed = ~(uint)0;

	//[SerializeField] private EnemyManager _enemyManager;

	//[SerializeField] private SongMeta songMeta;
	//[SerializeField] private TrackData trackData;

	private void Awake()
	{
		//EventManager.EventInitialise(EventType.SPAWN);	
	}

	void Start()
	{
		//if (instance == this) return;

		//if (instance != null)
		//{
		//	Destroy(this);
		//}
		//else
		//{
		//	instance = this;

		//	// Hardcoded value for prototype - remove when a system is in place
		//	// for selecting and passing a song/track to this manager
		//	StartSong(songMeta, trackData);
		//}
	}

	//public static void StartSong(SongMeta song, TrackData track)
	//{
	//	_cur_song = song;
	//	_cur_track = track;

	//	_beats_processed = 0;

	//	// Replace this with an event that triggers StartSong in SongManager
	//	//SongManager.StartSong(song);
	//}
}
