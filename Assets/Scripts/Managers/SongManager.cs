using UnityEngine;

public class SongManager : MonoBehaviour
{
	public static SongManager instance { get; private set; } = null;

	private static int _lastBeat;

	public AudioSource songSource;
	// Decides whether to call Beat events before the song starts
	public bool broadcastNegativeBeats = true;

	public static void StartSong(SongMeta song)
	{
		instance.songSource.Stop();

		instance.songSource.clip = song.clip;
		instance.songSource.Play();

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