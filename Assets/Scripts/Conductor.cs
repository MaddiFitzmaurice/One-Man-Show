using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
	public static Conductor instance { get; private set; }

	private static float _currentBPS = 2f;   // Assigned beats-per-second
	private static float _currentBPM = 120f; // Assigned beats-per-minute

	public static double startOffset { get; private set; } = 0.0; // The offset of the start of the song, in seconds
	public static double inputOffset = 0.2; // The offset between a keypress and being processed by the program, in seconds

	public static double startedTime { get; private set; } = 0.0;

	public static float rawSongTime { get; private set; } = 0f;
	public static float rawSongBeat { get; private set; } = 0f;
	public static int rawLastBeat { get; private set; } = 0;
	public static int rawNearestBeat { get; private set; } = 0;
	public static float rawBeatDelta { get; private set; } = 0f;

	// The current time/beat with input delay taken out
	public static float songTime { get; private set; } = 0f; // The current position in the song in seconds
	public static float songBeat { get; private set; } = 0f; // The current position in beats, can be between beats
	public static int lastBeat { get; private set; } = 0; // The number of the last beat, starting from 0 at the start of the song
	public static int nearestBeat { get; private set; } = 0; // The number of the closest beat, starting from 0 at the start of the song
	public static float beatDelta { get; private set; } = 0f; // Offset of the closest beat in seconds

	// Beats per second
	public static float currentBPS
	{
		get => _currentBPS;
		private set
		{
			_currentBPS = value;
			_currentBPM = value * 60f;
		}
	}

	// Beats per minute
	public static float currentBPM
	{
		get => _currentBPM;
		private set {
			_currentBPM = value;
			_currentBPS = value / 60f;
		}
	}

	// Sets the reference point for beats and time to be calculated relative to
	public static void StartTracking(float bpm, double startOffset)
	{
		currentBPM = bpm;
		Conductor.startOffset = startOffset;

		startedTime = AudioSettings.dspTime;

		rawSongTime = (float)(AudioSettings.dspTime - startedTime - startOffset);
		rawSongBeat = rawSongTime * currentBPS;
		rawLastBeat = Mathf.FloorToInt(rawSongBeat);
		rawNearestBeat = Mathf.RoundToInt(rawSongBeat);
		rawBeatDelta = (rawSongBeat - rawNearestBeat) / currentBPS;

		songTime = (float)(AudioSettings.dspTime - startedTime - startOffset - inputOffset);
		songBeat = songTime * currentBPS;
		lastBeat = Mathf.FloorToInt(songBeat);
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
		rawLastBeat = Mathf.FloorToInt(rawSongBeat);
		rawNearestBeat = Mathf.RoundToInt(rawSongBeat);
		rawBeatDelta = (rawSongBeat - rawNearestBeat) / currentBPS;

		songTime = (float)(AudioSettings.dspTime - startedTime - startOffset - inputOffset);
		songBeat = songTime * currentBPS;
		lastBeat = Mathf.FloorToInt(songBeat);
		nearestBeat = Mathf.RoundToInt(songBeat);
		beatDelta = (songBeat - nearestBeat) / currentBPS;
	}
}
