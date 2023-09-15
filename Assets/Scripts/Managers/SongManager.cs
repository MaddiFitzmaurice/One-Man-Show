using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SongManager : MonoBehaviour
{
	public static SongManager instance { get; private set; } = null;

	private static int _lastBeat;

	public AudioSource songSource;

	// Decides whether to call Beat events before the song starts
	public bool broadcastNegativeBeats = true;

	private static int LastBeat
	{
		get => _lastBeat;
		set
		{
			if (value <= _lastBeat) return;

			_lastBeat = value;

			if (value < 0 && !instance.broadcastNegativeBeats) return;

			EventManager.EventTrigger(EventType.BEAT, _lastBeat);
		}
	}

	public static void StartSong(SongMeta song, bool negativeBeats = true)
	{
		instance.songSource.Stop();

		instance.songSource.clip = song.clip;
		instance.songSource.Play();

		Debug.Log("Starting song with BPM " + song.BPM + " and offset " + song.startOffset);
		Conductor.StartTracking(song.BPM, song.startOffset);

		_lastBeat = Conductor.RawLastBeat;
		instance.broadcastNegativeBeats = negativeBeats;

		if (_lastBeat < 0 && !negativeBeats) return;

		EventManager.EventTrigger(EventType.BEAT, _lastBeat);
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this);
			return;
		}

		instance = this;
	}

	private void Start()
	{
		EventManager.EventInitialise(EventType.BEAT);
	}

	private void Update()
	{
		if (!songSource.isPlaying) return;

		LastBeat = Conductor.RawLastBeat;
	}
}