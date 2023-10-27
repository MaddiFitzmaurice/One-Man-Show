using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CalibrationManager : MonoBehaviour
{
	private Coroutine _calibrator = null;

	List<double> inputOffsets = new List<double>();

	public Transform arrowTransform;
	public double BPM = 120.0;
	public uint maxOffsets = 16;
	public uint beatsPerCycle = 16;
	public uint stopAfterNumBeats = 16;

	public TextMeshProUGUI startMessage;
	public TextMeshProUGUI pressMessage;
	public TextMeshProUGUI delayMessage;

	public AudioSource audioSource;

	public BeatLineController beatLine;

	public GameObject menuButton;

	public string delayMessageFormat = "Average delay: {0:N2}ms {1}";

	public void Start()
	{
		SetDelayText();
	}

	public void Update()
	{
		if (_calibrator != null) return;

		if (Input.GetKeyDown(KeyCode.Space)) StartCalibration();
	}

	public void StartCalibration()
	{
		StopCalibration();

		Debug.Log("Calibration started");

		Conductor.StartTracking(BPM, AudioSettings.dspTime, 0);

		startMessage.gameObject.SetActive(false);
		pressMessage.gameObject.SetActive(true);
		menuButton.SetActive(false);
		beatLine.enabled = true;

		_calibrator = StartCoroutine(CalibrationLoop(maxOffsets));
	}

	public void StopCalibration()
	{
		if (_calibrator == null) return;

		Debug.Log("Calibration cancelled");

		StopCoroutine(_calibrator);
		_calibrator = null;

		startMessage.gameObject.SetActive(true);
		pressMessage.gameObject.SetActive(false);
		menuButton.SetActive(true);
		beatLine.enabled = false;
	}

	private void SetDelayText()
	{
		if (delayMessage == null) return;

		string text;

		if (Conductor.InputOffset < 0)
		{
			text = string.Format(delayMessageFormat, -Conductor.InputOffset * 1000.0, "early");
		}
		else if (Conductor.InputOffset > 0)
		{
			text = string.Format(delayMessageFormat, Conductor.InputOffset * 1000.0, "late");
		}
		else
		{
			text = string.Format(delayMessageFormat, 0, "");
		}

		delayMessage.SetText(text.Trim());
	}

	public IEnumerator CalibrationLoop(uint offsets)
	{
		int lastBeat = 0;
		int lastPressedBeat = -1;

		audioSource.Play();

		if (offsets != 0)
		{
			while (inputOffsets.Count >= offsets)
			{
				inputOffsets.RemoveAt(0);
			}
		}

		if (Input.GetKeyDown(KeyCode.Space)) yield return null;

		while (Conductor.RawNearestBeat <= stopAfterNumBeats)
		{
			if (Conductor.RawLastBeat > lastBeat)
			{
				lastBeat = Conductor.RawLastBeat;
				audioSource.Play();
			}

			arrowTransform.rotation = Quaternion.Euler(0, 0, -360f * Mathf.Clamp(Conductor.RawSongBeat, 0, stopAfterNumBeats) / stopAfterNumBeats);

			if (lastBeat > lastPressedBeat && Input.GetKeyDown(KeyCode.Space))
			{
				lastPressedBeat = lastBeat;

				if (offsets != 0 && inputOffsets.Count == offsets)
				{
					inputOffsets.RemoveAt(0);
				}

				inputOffsets.Add(Conductor.RawBeatDelta);

				delayMessage.gameObject.SetActive(true);

				double avg = inputOffsets.Average();

				Conductor.InputOffset = avg;

				Debug.Log($"Set input delay to {Conductor.InputOffset * 1000.0}ms");

				SetDelayText();
			}

			yield return null;
		}

		Debug.Log("Calibration finished");

		arrowTransform.rotation = Quaternion.Euler(0, 0, 0);

		_calibrator = null;

		startMessage.gameObject.SetActive(true);
		pressMessage.gameObject.SetActive(false);
		menuButton.SetActive(true);
		beatLine.enabled = false;
	}
}
