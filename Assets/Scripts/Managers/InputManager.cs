using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, GameInput.IGameplayActions
{
    // Input
    [SerializeField]
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
        EventManager.EventInitialise(EventType.PARRY_LEFT);
        EventManager.EventInitialise(EventType.PARRY_RIGHT);
        EventManager.EventInitialise(EventType.PARRY_FORWARD);

        // TEST
        EventManager.EventInitialise(EventType.PLAYER_HIT);
    }

    public void OnLeftParry(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventManager.EventTrigger(EventType.PARRY_LEFT, null);
        }    
    }

    public void OnRightParry(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventManager.EventTrigger(EventType.PARRY_RIGHT, null);
        }
    }

    public void OnForwardParry(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // To test if it can take data
            EventManager.EventTrigger(EventType.PARRY_FORWARD, 1);
        }
    }

    public void OnTESTPLAYERHIT(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // To test player being hit
            EventManager.EventTrigger(EventType.PLAYER_HIT, null);
        }
    }
}
