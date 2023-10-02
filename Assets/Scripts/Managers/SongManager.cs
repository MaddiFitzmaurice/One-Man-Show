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
    [Min(0)]
    public double startDelay = 0.0;
    // If true, play metronome clicks on each beat (to test sync)
    [SerializeField] bool _debugging = false;

    // Metronome clicks for debugging
    [SerializeField] AudioClip _debugMetronome;

    public void StartSong(SongMeta song, bool negativeBeats = true)
    {
        if(_curSong != null)
        {
            StopSong();
        }

        broadcastNegativeBeats = negativeBeats;

        double start = AudioSettings.dspTime;

        _curSong = song;
        songSource.clip = song.clip;
        songSource.PlayScheduled(start + startDelay);

        Debug.Log($"Starting song at {song.BPM}BPM offset by {song.startOffset + startDelay}s");
        Conductor.StartTracking(song.BPM, start, song.startOffset + startDelay);

        StartCoroutine(BroadcastBeats());

        // if debugging, play metronome clicks on every beat
        if (_debugging)
        {
            EventManager.EventSubscribe(EventType.BEAT, DebugMetronome);
        }
    }

    public void StopSong()
    {
        if (_curSong == null) return;

        songSource.Stop();
        _doesSongLoop = false;
        _curSong = null;
    }

    private void Awake()
    {
        EventManager.EventInitialise(EventType.BEAT);
    }

    public void DebugMetronome(object data)
    {
        songSource.PlayOneShot(_debugMetronome);
    }

    IEnumerator BroadcastBeats()
    {
        float currentBeat = Conductor.RawLastBeat; // Get first beat

        // Broadcast until song finishes
        while (_curSong != null || _doesSongLoop)
        {
            // Wait for a half-beat passing before broadcasting again
            if (Conductor.RawSongBeat >= currentBeat + 0.5)
            {
                currentBeat += 0.5f;
                //Broadcast beat and reset current time
                EventManager.EventTrigger(EventType.BEAT, currentBeat);
            }
            else
            {
                yield return null;
            }
        }
    }
}