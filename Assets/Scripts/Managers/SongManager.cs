using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SongManager : MonoBehaviour
{
	// I think this shouldn't be a static instance because no other game object should be accessing it freely
	//public static SongManager instance { get; private set; } = null;

	private static int _lastBeat;

	public AudioSource songSource;

	// Decides whether to call Beat events before the song starts
	public bool broadcastNegativeBeats = true;

	// Song Meta
	[SerializeField] SongMeta _song;

	private int LastBeat
	{
		get => _lastBeat;
		set
		{
			if (value <= _lastBeat) return;

			_lastBeat = value;

			if (value < 0 && broadcastNegativeBeats) return;

			EventManager.EventTrigger(EventType.BEAT, _lastBeat);
		}
	}

	public void StartSong(SongMeta song, bool negativeBeats = true)
	{
		songSource.Stop();

		songSource.clip = song.clip;
		songSource.Play();

		Debug.Log("Starting song with BPM " + song.BPM + " and offset " + song.startOffset);
		Conductor.StartTracking(song.BPM, song.startOffset);

		_lastBeat = Conductor.RawLastBeat;
		broadcastNegativeBeats = negativeBeats;

		if (_lastBeat < 0 && !negativeBeats) return;
		StartCoroutine(BroadcastBeats());
	}

	/*private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this);
			return;
		}

		instance = this;
	}*/

	private void Start()
	{
		EventManager.EventInitialise(EventType.BEAT);

		// TrackManager will initiate StartSong, but for now call here
		StartSong(_song, false);
	}

	private void Update()
	{
		if (!songSource.isPlaying) return;

		LastBeat = Conductor.RawLastBeat;
	}

    IEnumerator BroadcastBeats()
    {
		// Broadcast starting beat
		EventManager.EventTrigger(EventType.BEAT, Conductor.RawSongBeat);
        Debug.Log("Current beat number: " + Conductor.RawSongBeat);

        // Calculate the total number of beats in the song
        double totalBeats = _song.clip.length * Conductor.CurrentBPS;
        Debug.Log("Total number of beats in song: " + totalBeats);

        double currentTime = Conductor.RawSongTime;

        // Broadcast until song finishes
        while (Conductor.RawSongBeat < totalBeats)
        {
            // Wait for the equivalent of a half-beat in seconds passing before broadcasting again
            if (Conductor.RawSongTime >= currentTime + 0.375f)
            {
				//Broadcast beat and reset current time
				EventManager.EventTrigger(EventType.BEAT, Conductor.RawSongBeat);
				Debug.Log("Current beat number: " + Conductor.RawSongBeat);
                currentTime = Conductor.RawSongTime;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}