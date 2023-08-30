using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // NOTE: JUST A TEST FOR SFXMANAGER. Player sound will always
    // come from centre
    [SerializeField] AudioClip _attackSFXForward;
    [SerializeField] AudioClip _attackSFXLeft;
    [SerializeField] AudioClip _attackSFXRight;
    SFXData _SFXDataForward;
    SFXData _SFXDataLeft;
    SFXData _SFXDataRight;

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

    private void Start()
    {
        _SFXDataForward = new SFXData(_attackSFXForward, StageDirection.FORWARD);
        _SFXDataRight = new SFXData(_attackSFXRight, StageDirection.RIGHT);
        _SFXDataLeft = new SFXData(_attackSFXLeft, StageDirection.LEFT);
    }

    // Callback functions for Parry events that accept data
    public void ParryLeftHandler(object data)
    {
        Debug.Log("Test Parry Left");
        SendSFXData(_SFXDataLeft);
    }

    public void ParryRightHandler(object data)
    {
        Debug.Log("Test Parry Right");
        SendSFXData(_SFXDataRight);
    }

    public void ParryForwardHandler(object data)
    {
        Debug.Log("Test Parry Forward " + (int)data);
        SendSFXData(_SFXDataForward);
    }

    private void SendSFXData(SFXData sfxData)
    {
        EventManager.EventTrigger(EventType.SFX, sfxData);
    }
}
