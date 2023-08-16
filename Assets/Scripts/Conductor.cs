using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
	public static Conductor instance { get; private set; }

	private float _currentBPS = 2f;
	private float _currentBPM = 120f;

	public double startOffset { get; private set; } = 0.0;
	public double inputOffset = 0.2;

	public double startedTime { get; private set; } = 0.0;

	public float rawSongTime { get; private set; } = 0f;
	public float rawSongBeat { get; private set; } = 0f;
	public int rawBeatNum { get; private set; } = 0;

	// The current time/beat with input delay taken out
	public float songTime { get; private set; } = 0f;
	public float songBeat { get; private set; } = 0f;
	public int beatNum { get; private set; } = 0;

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
		rawBeatNum = Mathf.FloorToInt(rawSongBeat);

		songTime = (float)(AudioSettings.dspTime - startedTime - startOffset - inputOffset);
		songBeat = songTime * currentBPS;
		beatNum = Mathf.FloorToInt(songBeat);
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
		rawBeatNum = Mathf.FloorToInt(rawSongBeat);

		songTime = (float)(AudioSettings.dspTime - startedTime - startOffset - inputOffset);
		songBeat = songTime * currentBPS;
		beatNum = Mathf.FloorToInt(songBeat);
	}
}
