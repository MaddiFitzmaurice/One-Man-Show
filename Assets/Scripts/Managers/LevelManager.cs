using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<SpawnData> _levelLayout;
    private float _currentBeat;

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.BEAT, BeatHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.BEAT, BeatHandler);
    }

    private void Awake()
    {
        EventManager.EventInitialise(EventType.SPAWN);
    }

    // Update current beat
    public void BeatHandler(object data)
    {
        _currentBeat = (float)data;

        CheckBeat();
    }

    public void CheckBeat()
    {
        // loop is needed if spawning multiple enemies on same beat
        while (true)
        {
            if (_levelLayout.Count > 0)
            {
                if (_currentBeat > _levelLayout[0].Beat)
                {
                    EventManager.EventTrigger(EventType.SPAWN, _levelLayout[0]);
                    _levelLayout.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
            else
			{
                break;
			}
        }
    }
}
