using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float _hitWindow = 0.08f; // ms leniency in both directions (consistent across all enemies)
    private float _earlyWindow; // how many beats the player can be early
    private float _lateWindow; // how many beats the player can be late
    [SerializeField] private StageDirection _direction;

    // Start is called before the first frame update
    void Start()
    {
        Conductor cd = Conductor.instance;
        float beatDiff = cd.currentBPS * _hitWindow;
        _earlyWindow = (_hitTime + _startBeat) - beatDiff;
        _lateWindow = (_hitTime + _startBeat) + beatDiff;
    }
    
    // set unique values for this enemy
    public void Initialise(StageDirection dr, float sb)
	{
        _startBeat = sb;
        _direction = dr;
	}

    // perform movement and animations
    void Update()
    {
        Conductor cd = Conductor.instance;
        float relativeBeat = cd.songBeat - _startBeat;
        List<EnemyBeat> deleteBeats = new List<EnemyBeat>();

        // check if any new beats/animations need to occur
        foreach (EnemyBeat b in _beats)
		{
            if (relativeBeat > b.BeatOffset)
			{
                // TODO: play the sound associated with this beat (ideally skipping partway into the sound based on time difference)
                deleteBeats.Add(b); // queue the beat for deletion
			}
		}

        // delete all occurred beats
        foreach (EnemyBeat b in deleteBeats)
		{
            _beats.Remove(b);
		}

        // check if it's time to attack
        if (_earlyWindow < cd.songBeat && cd.songBeat > _lateWindow)
		{
            switch (_direction)
			{
                case StageDirection.LEFT:
                    // TODO: it should be something like this?? this gives an error tho
                    // System.Action<object> a = () => Destroy(this); 
                    // EventManager.EventSubscribe("Parry_Left", a);
                    break;
                case StageDirection.RIGHT:
                    // subscribe this enemy dying to EVENT_PARRY_RIGHT
                    break;
                case StageDirection.FORWARD:
                    // subscribe this enemy dying to EVENT_PARRY_RIGHT
                    break;
            }
        }
        // if the player hasn't destroyed this enemy in time, deal damage
        else if (cd.songBeat > _lateWindow)
		{
            // TODO: make this deal damage
		}
    }
}
