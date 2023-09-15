using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
	private static TrackManager instance = null;

	private static TrackData _cur_track = null;
	private static SongMeta _cur_song = null;
	
	private static uint _beats_processed = ~(uint)0;

	[SerializeField] private EnemyManager _enemyManager;

	[SerializeField] private SongMeta songMeta;
	[SerializeField] private TrackData trackData;

	void Start()
	{
		if (instance == this) return;

		if (instance != null)
		{
			Destroy(this);
		}
		else
		{
			instance = this;

			// Hardcoded value for prototype - remove when a system is in place
			// for selecting and passing a song/track to this manager
			StartSong(songMeta, trackData);
		}
	}

	public static void StartSong(SongMeta song, TrackData track)
	{
		_cur_song = song;
		_cur_track = track;

		_beats_processed = 0;

		SongManager.StartSong(song);
	}
}
