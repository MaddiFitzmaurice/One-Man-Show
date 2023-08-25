using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.EventSubscribe("Parry_Left", ParryLeftHandler);
        EventManager.EventSubscribe("Parry_Right", ParryRightHandler);
        EventManager.EventSubscribe("Parry_Forward", ParryForwardHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe("Parry_Left", ParryLeftHandler);
        EventManager.EventUnsubscribe("Parry_Right", ParryRightHandler);
        EventManager.EventUnsubscribe("Parry_Forward", ParryForwardHandler);
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
