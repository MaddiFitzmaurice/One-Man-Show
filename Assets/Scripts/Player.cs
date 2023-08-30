using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PARRY_LEFT, ParryLeftHandler);
        EventManager.EventSubscribe(EventType.PARRY_RIGHT, ParryRightHandler);
        EventManager.EventSubscribe(EventType.PARRY_FORWARD, ParryForwardHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PARRY_LEFT, ParryLeftHandler);
        EventManager.EventUnsubscribe(EventType.PARRY_RIGHT, ParryRightHandler);
        EventManager.EventUnsubscribe(EventType.PARRY_FORWARD, ParryForwardHandler);
    }

    // Callback functions for Parry events that accept data
    public void ParryLeftHandler(object data)
    {
        Debug.Log("Test Parry Left");
    }

    public void ParryRightHandler(object data)
    {
        Debug.Log("Test Parry Right");
    }

    public void ParryForwardHandler(object data)
    {
        Debug.Log("Test Parry Forward " + (int)data);
    }
}
