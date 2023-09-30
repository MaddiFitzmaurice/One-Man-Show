using System;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
	private static TrackManager instance = null;

	private static TrackBeat[] _cur_track = null;
	private static SongMeta _cur_song = null;
	private static uint _cur_beat = 0;

	[SerializeField] private TrackData trackData;
	[SerializeField] private SongManager songManager;

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
			StartSong(trackData);
		}
	}

	public static void StartSong(TrackData track)
	{
		_cur_song = track.song;
		_cur_track = (TrackBeat[])track.beats.Clone(); // Take a deep copy so that the track is not modified
		_cur_beat = 0;

		Array.Sort(_cur_track); // Ensure that the beat order is correct

		instance.songManager.StartSong(track.song);
	}

	public void Update()
	{
		if (_cur_track == null) return;
		// No more beats
		if (_cur_beat >= _cur_track.Length) return;

		float beat = Conductor.RawSongBeat;

		while (beat >= _cur_track[_cur_beat].beat)
		{
            foreach (EnemySpawn _enemy in _cur_track[_cur_beat].enemies)
			{
				SpawnData spawn = new SpawnData(_cur_track[_cur_beat].beat, _enemy.direction, _enemy.type);
				EventManager.EventTrigger(EventType.SPAWN, spawn);
			}

			foreach (SpawnEvent _event in _cur_track[_cur_beat].events)
			{
				EventManager.EventTrigger(_event.type, _event.data);
			}

			_cur_beat++;

			if (_cur_beat >= _cur_track.Length) return;
		}
	}
}
