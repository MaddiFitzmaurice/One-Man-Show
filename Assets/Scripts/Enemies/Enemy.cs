using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyBeat
{
    public float BeatOffset;
    public AudioClip Sound;
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _startBeat;
    // private List<Animation> _enemyAnims; // this may not work when changing directions? not needed for now
    [SerializeField] private List<EnemyBeat> _beats;

    [SerializeField] private float _hitTime; // how many beats until this enemy attacks
    [SerializeField] private AudioClip _hitSound; // the sound this enemy plays if it attacks the player
    [SerializeField] private AudioClip _deathSound; // the sound this enemy plays if it is killed
    private float _hitWindow = 0.08f; // ms leniency in both directions (consistent across all enemies)
    private float _earlyWindow; // how many beats the player can be early
    private float _lateWindow; // how many beats the player can be late
    [SerializeField] private StageDirection _direction;

    // Start is called before the first frame update
    void Start()
    {
        float beatDiff = Conductor.currentBPS * _hitWindow;
        _earlyWindow = (_hitTime + _startBeat) - beatDiff;
        _lateWindow = (_hitTime + _startBeat) + beatDiff;
    }
    
    // set unique values for this enemy
    public void Initialise(StageDirection dr, float sb)
	{
        _startBeat = sb;
        _direction = dr;
	}

	private void OnDisable()
	{
        switch (_direction)
        {
            case StageDirection.LEFT:
                EventManager.EventUnsubscribe(EventType.PARRY_LEFT, DefeatMe);
                break;
            case StageDirection.RIGHT:
                EventManager.EventUnsubscribe(EventType.PARRY_RIGHT, DefeatMe);
                break;
            case StageDirection.FORWARD:
                EventManager.EventUnsubscribe(EventType.PARRY_FORWARD, DefeatMe);
                break;
        }
    }

	// perform movement and animations
	void Update()
    {
        float relativeBeat = Conductor.songBeat - _startBeat;
        List<EnemyBeat> deleteBeats = new List<EnemyBeat>();

        // check if any new beats/animations need to occur
        foreach (EnemyBeat b in _beats)
		{
            if (relativeBeat > b.BeatOffset)
			{
                // TODO: play the sound associated with this beat (ideally skipping partway into the sound based on time difference)
                SFXData thisSound = new SFXData(b.Sound, _direction);
                EventManager.EventTrigger(EventType.SFX, thisSound);
                deleteBeats.Add(b); // queue the beat for deletion
			}
		}

        // delete all occurred beats
        foreach (EnemyBeat b in deleteBeats)
		{
            _beats.Remove(b);
		}

        // check if it's time to attack
        if (_earlyWindow < Conductor.songBeat && Conductor.songBeat < _lateWindow)
		{
            switch (_direction)
			{
                case StageDirection.LEFT:
                    // subscribe this enemy dying to EVENT_PARRY_LEFT
                    EventManager.EventSubscribe(EventType.PARRY_LEFT, DefeatMe);
                    break;
                case StageDirection.RIGHT:
                    // subscribe this enemy dying to EVENT_PARRY_RIGHT
                    EventManager.EventSubscribe(EventType.PARRY_RIGHT, DefeatMe);
                    break;
                case StageDirection.FORWARD:
                    // subscribe this enemy dying to EVENT_PARRY_FORWARD
                    EventManager.EventSubscribe(EventType.PARRY_FORWARD, DefeatMe);
                    break;
            }
        }
        // if the player hasn't destroyed this enemy in time, deal damage
        else if (Conductor.songBeat > _lateWindow)
		{
            // TODO: make this deal damage
            SFXData hitClip = new SFXData(_hitSound, StageDirection.FORWARD);
            EventManager.EventTrigger(EventType.SFX, hitClip);
            gameObject.SetActive(false);
        }
    }

    public void DefeatMe(object data)
	{
        // TODO: any death animations/sounds go here
        gameObject.SetActive(false);
	}
}
