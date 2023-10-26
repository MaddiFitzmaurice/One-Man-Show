using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CalibrationManager : MonoBehaviour, GameInput.ICalibrationActions
{
    private GameInput _inputActions;
    private Coroutine _calibrator = null;
	int _pressedBeat = -1;

	private List<double> inputOffsets = new List<double>();

	[SerializeField] private SceneSwitcher switcher;

	[SerializeField] private Transform arrowTransform;
	[SerializeField] private double BPM = 120.0;
	[SerializeField] private uint maxOffsets = 16;
	[SerializeField] private uint stopAfterNumBeats = 16;

	[SerializeField] private TextMeshProUGUI startMessage;
	[SerializeField] private TextMeshProUGUI pressMessage;
	[SerializeField] private TextMeshProUGUI delayMessage;

	[SerializeField] private AudioSource audioSource;

	[SerializeField] private BeatLineController beatLine;

	public GameObject menuButton;

	public string delayMessageFormat = "Average delay: {0:N2}ms {1}";

	private void Awake()
	{
		if (_inputActions != null) return;
		_inputActions = new GameInput();
		_inputActions.Calibration.SetCallbacks(this);
		_inputActions.Calibration.Enable();
	}

	private void OnEnable()
	{
		if (_inputActions != null) _inputActions.Enable();
	}

	private void OnDisable()
	{
		if (_calibrator != null) StopCalibration();
		if (_inputActions != null) _inputActions.Disable();
	}

	public void Start()
	{
		SetDelayText();
	}

	public void OnBeat(InputAction.CallbackContext context)
	{
		if (_calibrator == null)
		{
			StartCalibration();
			return;
		}

		int beat = Conductor.RawNearestBeat;

		if (beat < _pressedBeat) return;

		_pressedBeat = beat;

		if (maxOffsets != 0 && inputOffsets.Count == maxOffsets)
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

	public void OnBack(InputAction.CallbackContext context)
	{
		if (switcher != null) ToMenu();
    }

	public void ToMenu()
    {
        switcher.SetScene("TitleScene");
        StopCalibration();
        enabled = false;
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

		_calibrator = StartCoroutine(CalibrationLoop());
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

	public IEnumerator CalibrationLoop()
	{
		int lastBeepbeat = 0;

        _pressedBeat = -1;

		audioSource.Play();

		if (maxOffsets != 0)
		{
			while (inputOffsets.Count >= maxOffsets)
			{
				inputOffsets.RemoveAt(0);
			}
		}

		while (Conductor.RawLastBeat <= stopAfterNumBeats)
		{
			if (Conductor.RawLastBeat > lastBeepbeat)
			{
                lastBeepbeat = Conductor.RawLastBeat;
				audioSource.Play();
			}

			arrowTransform.rotation = Quaternion.Euler(0, 0, -360f * Mathf.Clamp(Conductor.RawSongBeat, 0, stopAfterNumBeats) / stopAfterNumBeats);

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
