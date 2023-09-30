using System.Collections;
using UnityEngine;

public class SongManager : MonoBehaviour
{
	private static SongMeta _curSong = null;

	public AudioSource songSource;

	// Whether the song loops (true during tutorials)
	[SerializeField] private bool _doesSongLoop = false;

	// Decides whether to call Beat events before the song starts
	public bool broadcastNegativeBeats = true;
	// If true, play metronome clicks on each beat (to test sync)
	[SerializeField] bool _debugging = false;

	// Metronome clicks for debugging
	[SerializeField] AudioClip _debugMetronome;

	public void StartSong(SongMeta song, bool negativeBeats = true)
	{
		songSource.Stop();

		_curSong = song;
		songSource.clip = song.clip;

        Debug.Log("Starting song with BPM " + song.BPM + " and offset " + song.startOffset);
        Conductor.StartTracking(song.BPM, song.startOffset);

        songSource.Play();
        StartCoroutine(BroadcastBeats());

		broadcastNegativeBeats = negativeBeats;

		// if debugging, play metronome clicks on every beat
		if (_debugging)
		{
			EventManager.EventSubscribe(EventType.BEAT, DebugMetronome);
		}
	}

	private void Start()
	{
		EventManager.EventInitialise(EventType.BEAT);
	}

	public void DebugMetronome(object data)
	{
		songSource.PlayOneShot(_debugMetronome);
	}

    IEnumerator BroadcastBeats()
    {
        // Calculate the total number of beats in the song
        //double clipLength = _curSong.clip.length;
		//Debug.Log("Total number of beats in song: " + clipLength * Conductor.CurrentBPS);

		float currentBeat = Conductor.LastBeat; // Get first beat

		// Broadcast starting beat
		EventManager.EventTrigger(EventType.BEAT, currentBeat);
		//Debug.Log("Current beat number: " + Conductor.RawSongBeat);

        // Broadcast until song finishes
        while (songSource.isPlaying || _doesSongLoop)
        {
            // Wait for a half-beat passing before broadcasting again
            if (Conductor.RawSongTime >= currentBeat + 0.5)
            {
				//Broadcast beat and reset current time
				EventManager.EventTrigger(EventType.BEAT, currentBeat);
				//Debug.Log("Current beat number: " + Conductor.RawSongBeat);
				currentBeat += 0.5f;
				//Debug.Log(currentTime);
            }
            else
            {
				yield return null;
            }
        }
    }
}