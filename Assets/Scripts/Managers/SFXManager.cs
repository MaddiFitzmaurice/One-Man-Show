using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] AudioSource _leftCue;
    [SerializeField] AudioSource _forwardCue;
    [SerializeField] AudioSource _rightCue;

    Dictionary<StageDirection, AudioSource> _cues;

    private void Awake()
    {
        EventManager.EventInitialise(EventType.SFX);
        InitCues();
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.SFX, SFXEventHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.SFX, SFXEventHandler);
    }

    private void InitCues()
    {
        _cues = new Dictionary<StageDirection, AudioSource>
        {
            { StageDirection.LEFT, _leftCue },
            { StageDirection.FORWARD, _forwardCue },
            { StageDirection.RIGHT, _rightCue }
        };
    }

    // Handles SFXEvent with incoming SFX data to play at specified cue source
    public void SFXEventHandler(object data)
    {
        if (data == null) return;

        SFXData sfxData = data as SFXData;
        if (_cues.ContainsKey(sfxData.dir))
        {
            _cues[sfxData.dir].PlayOneShot(sfxData.clip);
        }
    }
}
