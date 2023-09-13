using System.Collections;
using UnityEngine;

public class SongManager : MonoBehaviour
{
	public static SongManager instance { get; private set; } = null;

	private static int _lastBeat;

	public AudioSource songSource;
	public SongMeta song; // is there a better place to store this?

	// Decides whether to call Beat events before the song starts
	public bool broadcastNegativeBeats = true;

	public static void StartSong(SongMeta song)
	{
		instance.songSource.Stop();

		instance.songSource.clip = song.clip;
		instance.songSource.Play();

		Debug.Log("Starting song with BPM " + song.BPM + " and offset " + song.startOffset);
		Conductor.StartTracking(song.BPM, song.startOffset);

		_lastBeat = Conductor.rawLastBeat;
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
		//SongMeta sm = new SongMeta();
		//sm.BPM = 85;
		//sm.clip = songSource.clip; // using the source's clip for now
		//sm.startOffset = 1.442f; // in milliseconds
		StartSong(song);
	}

	private void Update()
	{
		if (!songSource.isPlaying) return;
		if (Conductor.rawLastBeat <= _lastBeat) return;

		_lastBeat = Conductor.rawLastBeat;

		if (!broadcastNegativeBeats && _lastBeat < 0) return;

		EventManager.EventTrigger(EventType.BEAT, _lastBeat);
	}
}