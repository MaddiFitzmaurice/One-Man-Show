using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
	public static Conductor instance { get; private set; }

	private float _currentBPS = 2f;   // Assigned beats-per-second
	private float _currentBPM = 120f; // Assigned beats-per-minute

	public double startOffset { get; private set; } = 0.0; // The offset of the start of the song, in seconds
	public double inputOffset = 0.2; // The offset between a keypress and being processed by the program, in seconds

	public double startedTime { get; private set; } = 0.0;

	public float rawSongTime { get; private set; } = 0f;
	public float rawSongBeat { get; private set; } = 0f;
	public int rawNearestBeat { get; private set; } = 0;
	public float rawBeatDelta { get; private set; } = 0;

	// The current time/beat with input delay taken out
	public float songTime { get; private set; } = 0f; // The current position in the song in seconds
	public float songBeat { get; private set; } = 0f; // The current position in beats, can be between beats
	public int nearestBeat { get; private set; } = 0; // The number of the closest beat, starting from 0
	public float beatDelta { get; private set; } = 0; // Offset of the closest beat in seconds

	// Beats per second
	public float currentBPS
	{
		get => _currentBPS;
		private set
		{
			_currentBPS = value;
			_currentBPM = value * 60f;
		}
	}

	// Beats per minute
	public float currentBPM
	{
		get => _currentBPM;
		private set {
			_currentBPM = value;
			_currentBPS = value / 60f;
		}
	}

	public void StartTracking(float bpm, double startOffset)
	{
		currentBPM = bpm;
		this.startOffset = startOffset;

		startedTime = AudioSettings.dspTime;

		rawSongTime = (float)(AudioSettings.dspTime - startedTime - startOffset);
		rawSongBeat = rawSongTime * currentBPS;
		rawNearestBeat = Mathf.RoundToInt(rawSongBeat);
		rawBeatDelta = (rawSongBeat - rawNearestBeat) / currentBPS;

		songTime = (float)(AudioSettings.dspTime - startedTime - startOffset - inputOffset);
		songBeat = songTime * currentBPS;
		nearestBeat = Mathf.RoundToInt(songBeat);
		beatDelta = (songBeat - nearestBeat) / currentBPS;
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

	private void Update()
	{
		rawSongTime = (float)(AudioSettings.dspTime - startedTime - startOffset);
		rawSongBeat = rawSongTime * currentBPS;
		rawNearestBeat = Mathf.RoundToInt(rawSongBeat);
		rawBeatDelta = (rawSongBeat - rawNearestBeat) / currentBPS;

		songTime = (float)(AudioSettings.dspTime - startedTime - startOffset - inputOffset);
		songBeat = songTime * currentBPS;
		nearestBeat = Mathf.RoundToInt(songBeat);
		beatDelta = (songBeat - nearestBeat) / currentBPS;
	}
}
