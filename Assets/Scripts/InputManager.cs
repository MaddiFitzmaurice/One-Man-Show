using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, GameInput.IGameplayActions
{
    // Event Name Keys
    // TODO: Need to think of a better way to do keys - public enums maybe??? 
    private const string EVENT_PARRY_LEFT = "Parry_Left";
    private const string EVENT_PARRY_RIGHT = "Parry_Right";
    private const string EVENT_PARRY_FORWARD = "Parry_Forward";

    // Input
    private GameInput _inputActions;

    private void Awake()
    {
        InputInit();
        EventInit();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    // Set up input
    public void InputInit()
    {
        _inputActions = new GameInput();
        _inputActions.Gameplay.SetCallbacks(this);
        _inputActions.Gameplay.Enable();
    }

    // Create events
    public void EventInit()
    {
        EventManager.EventInitialise(EVENT_PARRY_LEFT);
        EventManager.EventInitialise(EVENT_PARRY_RIGHT);
        EventManager.EventInitialise(EVENT_PARRY_FORWARD);
    }

    public void OnLeftParry(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventManager.EventTrigger(EVENT_PARRY_LEFT, null);
        }    
    }

    public void OnRightParry(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventManager.EventTrigger(EVENT_PARRY_RIGHT, null);
        }
    }

    public void OnForwardParry(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // To test if it can take data
            EventManager.EventTrigger(EVENT_PARRY_FORWARD, 1);
        }
    }
}
