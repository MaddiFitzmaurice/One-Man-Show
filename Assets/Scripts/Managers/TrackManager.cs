using System;
using System.Collections;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
	private static TrackManager instance = null;

	private static SpawnBeat[] _cur_enemies = null;
	private static EventBeat[] _cur_events = null;
	private static SongMeta _cur_song = null;
	private static uint _cur_enemy_beat = 0;
	private static uint _cur_event_beat = 0;

	[SerializeField] private TrackData trackData;
	[SerializeField] private SongManager songManager;

	void Awake()
	{
		if (instance == this) return;

		if (instance != null)
		{
			Destroy(this);
		}
		else
		{
			instance = this;
		}
	}

	private void Start()
	{
		// Hardcoded value for prototype - remove when a system is in place
		// for selecting and passing a song/track to this manager
		StartSong(trackData);
	}

	public static void StartSong(TrackData track)
	{
		_cur_song = track.song;
		_cur_enemy_beat = 0;
		_cur_event_beat = 0;

		if (track.enemies.Length > 0)
		{
			_cur_enemies = new SpawnBeat[track.enemies.Length];
			track.enemies.CopyTo(_cur_enemies, 0);
			Array.Sort(_cur_enemies); // Ensure that the beat order is correct
		}
		else
		{
			_cur_enemies = null;
		}

		if (track.events.Length > 0)
		{
			_cur_events= new EventBeat[track.events.Length];
			track.events.CopyTo(_cur_events, 0);
			Array.Sort(_cur_events);
		}
		else
		{
			_cur_events = null;
		}

		instance.songManager.StartSong(_cur_song);
		instance.StartCoroutine(ProcessEnemies());
		instance.StartCoroutine(ProcessEvents());
	}

	public static IEnumerator ProcessEnemies()
	{
		if (_cur_enemies == null) yield break;
		if (_cur_enemy_beat >= _cur_enemies.Length) yield break;

		for (;;)
		{
			float beat = Conductor.RawSongBeat;

			while (beat >= _cur_enemies[_cur_enemy_beat].beat)
			{
				SpawnData spawn = new SpawnData(
					_cur_enemies[_cur_enemy_beat].beat,
					_cur_enemies[_cur_enemy_beat].direction,
					_cur_enemies[_cur_enemy_beat].type
				);

				EventManager.EventTrigger(EventType.SPAWN, spawn);

				_cur_enemy_beat++;

				if (_cur_enemy_beat >= _cur_enemies.Length) yield break;
			}

			yield return null;
		}
	}
	public static IEnumerator ProcessEvents()
	{
		if (_cur_events == null) yield break;
		if (_cur_event_beat >= _cur_events.Length) yield break;

		for(;;)
		{
			float beat = Conductor.RawSongBeat;

			while (beat >= _cur_events[_cur_event_beat].beat)
			{
				EventManager.EventTrigger(
					_cur_events[_cur_event_beat].type,
					_cur_events[_cur_event_beat].data
				);

				_cur_event_beat++;

				if (_cur_event_beat >= _cur_events.Length) yield break;
			}

			yield return null;
		}
	}
}
