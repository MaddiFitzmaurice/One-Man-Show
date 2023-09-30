using UnityEngine;

public class SongTester : MonoBehaviour
{
	public TrackData track;
	public AudioSource clickSource;

	int lastBeat;

	private void Start()
	{
		//TrackManager.StartSong(track);

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
