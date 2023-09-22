using System.Collections;
using UnityEngine;

public class SongManager : MonoBehaviour
{
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
		}
	}

	public void StartSong(SongMeta song, bool negativeBeats = true)
	{
		songSource.Stop();

		songSource.clip = song.clip;

        Debug.Log("Starting song with BPM " + song.BPM + " and offset " + song.startOffset);
        Conductor.StartTracking(song.BPM, song.startOffset);

        songSource.Play();
        StartCoroutine(BroadcastBeats());

        _lastBeat = Conductor.RawLastBeat;
		broadcastNegativeBeats = negativeBeats;

		if (_lastBeat < 0 && !negativeBeats) return;
	}

	private void Start()
	{
		EventManager.EventInitialise(EventType.BEAT);

		StartSong(_song, false);
	}

	private void Update()
	{
		if (!songSource.isPlaying) return;

		LastBeat = Conductor.RawLastBeat;
	}

    IEnumerator BroadcastBeats()
    {
        // Calculate the total number of beats in the song
        double totalBeats = _song.clip.length * Conductor.CurrentBPS;
        //Debug.Log("Total number of beats in song: " + totalBeats);

        // Broadcast starting beat
        EventManager.EventTrigger(EventType.BEAT, Conductor.RawSongBeat);
        //Debug.Log("Current beat number: " + Conductor.RawSongBeat);

        double currentTime = Conductor.RawSongTime;

        // Broadcast until song finishes
        while (Conductor.RawSongBeat < totalBeats)
        {
            // Wait for the equivalent of a half-beat in seconds passing before broadcasting again
            if (Conductor.RawSongTime >= currentTime + (Conductor.SecondsPerBeat / 2))
            {
				//Broadcast beat and reset current time
				EventManager.EventTrigger(EventType.BEAT, Conductor.RawSongBeat);
				//Debug.Log("Current beat number: " + Conductor.RawSongBeat);
                currentTime = Conductor.RawSongTime;
            }
            else
            {
				yield return null;
            }
        }
    }
}