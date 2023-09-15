using UnityEngine;

public class SongTester : MonoBehaviour
{
	public SongMeta song;
	public TrackData track;
	public AudioSource clickSource;

	int lastBeat;

	private void Start()
	{
		TrackManager.StartSong(song, track);

		lastBeat = Mathf.FloorToInt(Conductor.LastBeat);
	}

	// Update is called once per frame
	void Update()
    {
		if (lastBeat == Conductor.LastBeat) return;

		lastBeat = Conductor.LastBeat;

		clickSource.Play();
    }
}
