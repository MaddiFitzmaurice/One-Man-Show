using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, GameInput.IGameplayActions
{
	// Input
	[SerializeField]
	private GameInput _inputActions;
	[SerializeField,Min(0)]
	private double _inputBeatSeparation = 0.25; // Quarter beat
	[SerializeField, Min(0)]
	private double _parryMissPunishment = 0.5; // Half beat

	private double _timeRemaining = 0;

	private void Awake()
	{
		InputInit();
		EventInit();
	}

	private void OnEnable()
	{
		if (_inputActions != null) _inputActions.Enable();
		EventManager.EventSubscribe(EventType.PARRY_MISS, HandleMiss);
	}

	private void OnDisable()
	{
		if (_inputActions != null) _inputActions.Disable();
		EventManager.EventUnsubscribe(EventType.PARRY_MISS, HandleMiss);
	}

	private void HandleMiss(object data)
	{
		_timeRemaining = _parryMissPunishment / Conductor.CurrentBPS;
	}

	// Set up input
	public void InputInit()
	{
		if (_inputActions != null) return;
		_inputActions = new GameInput();
		_inputActions.Gameplay.SetCallbacks(this);
		_inputActions.Gameplay.Enable();
	}

	// Create events
	public void EventInit()
	{
		EventManager.EventInitialise(EventType.PARRY_INPUT);
	}

	public void OnLeftParry(InputAction.CallbackContext context)
	{
		if (_timeRemaining != 0) return;

		if (context.started)
		{
			_timeRemaining = _inputBeatSeparation / Conductor.CurrentBPS;
			EventManager.EventTrigger(EventType.PARRY_INPUT, StageDirection.LEFT);
		}
	}

	public void OnRightParry(InputAction.CallbackContext context)
	{
		if (_timeRemaining > 0) return;

		if (context.started)
		{
			_timeRemaining = _inputBeatSeparation / Conductor.CurrentBPS;
			EventManager.EventTrigger(EventType.PARRY_INPUT, StageDirection.RIGHT);
		}
	}

	public void OnForwardParry(InputAction.CallbackContext context)
	{
		if (_timeRemaining > 0) return;

		if (context.started)
		{
			_timeRemaining = _inputBeatSeparation / Conductor.CurrentBPS;
			// To test if it can take data
			EventManager.EventTrigger(EventType.PARRY_INPUT, StageDirection.FORWARD);
		}
	}

	private void Update()
	{
		_timeRemaining = Math.Max(0.0, _timeRemaining - Time.deltaTime);
	}
}
