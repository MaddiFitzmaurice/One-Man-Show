using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct MoveBeat
{
	[Min(0)]
	public float beat;
	[Range(0, 1)]
	public float percentage;
}

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
	[HideInInspector] public EnemyManager manager;

	[Header("Beat settings")]
	[SerializeField] private EnemyBeat[] _setBeats; // the defined list of beats this enemy type has
	private List<EnemyBeat> _beats; // the current list of beats of this specific enemy

	[SerializeField] private float _hitTime; // how many beats until this enemy attacks
	[SerializeField] private float _hitWindow = 0.098f; // seconds leniency in both directions (consistent across all enemies)

	[Header("Movement")]
	[SerializeField] private MoveBeat[] _moveBeats;
	[SerializeField] public Vector3 _rightSpawnOffset = Vector3.zero;
	[SerializeField] public Vector3 _forwardSpawnOffset = Vector3.zero;
	[SerializeField] private AnimationCurve _movementAnimation;
    private Vector3 _startPosition;
	private Vector3 _endPosition;

    [Header("Visuals")]
    [SerializeField] private GameObject _spawnOnDeath = null;
    [SerializeField] private SpriteRenderer _sprite;
	[SerializeField] private AnimatorOverrideController frontOverride;
	[SerializeField] private AnimatorOverrideController sideOverride;
	private Animator _anim;

    [Header("Tutorial")]
    [SerializeField] private GameObject _tutorialIndicatorPrefab; // the object to spawn to show timings during the tutorial
    private GameObject _tutorialIndicator; // where the instance of the tutorial indicator is stored
    private bool _tutorialMode = false; // when true, show a red flash for the correct timing

    [Header("SFX")]
    [SerializeField] private AudioClipData _hitClip; // the sound this enemy plays if it attacks the player
    [SerializeField] private AudioClipData _deathClip; // the sound this enemy plays if it is killed
    [SerializeField] private AudioClipData _guideClip; // the sound that plays at the perfect hit timing
    private SFXData _deathSFX = null;
    private SFXData _hitSFX = null;
    private SFXData _guideSFX = null;

    // Coroutines
    private Coroutine _moveCoroutine = null;
    private Coroutine _attackCoroutine = null;

    // Beat Tracking
    private float _currentBeat;
	private float _startBeat;
	private float _earlyWindow; // how many beats the player can be early
	private float _lateWindow; // how many beats the player can be late

	private StageDirection _direction;

	// Accel
	//private AnimationCurve _accelCurve;
	//private float _maxAccel;
	//private float _travelDistance;
	//private float _moveIntervalDist; // How far in units an Enemy has to move

	private void Awake()
	{
		if (_deathClip != null) _deathSFX = new SFXData(_deathClip, StageDirection.FORWARD);
		if (_hitClip   != null) _hitSFX   = new SFXData(_hitClip  , StageDirection.FORWARD);
        if (_guideClip != null) _guideSFX = new SFXData(_guideClip, StageDirection.FORWARD);
        _anim = GetComponent<Animator>();
		_beats = new List<EnemyBeat>(); // just so it doesn't error out when the game starts
	}

	// set unique values for this enemy
	public void Initialise(StageDirection dr, float sb, Vector3 startPos, Vector3 endPos, bool tutorial)
	{
		_sprite.color = Color.white;
		_startBeat = sb;
		_direction = dr;
		_tutorialMode = tutorial;

		SetAnimDirection();

		_startPosition = startPos;
		_endPosition = endPos;
		//_travelDistance = Vector3.Distance(_startPosition, _endPosition);
		//_moveIntervalDist = _travelDistance / _setBeats.Count;

		if(_moveCoroutine != null) StopCoroutine(_moveCoroutine);

        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }

        transform.position = startPos;

		float beatDiff = _hitWindow * (float)Conductor.CurrentBPS; // what percentage of a beat the hit window falls within
		_earlyWindow = (_hitTime + _startBeat) - beatDiff;
		_lateWindow = (_hitTime + _startBeat) + beatDiff;
		Debug.Log($"Early window is beat {_earlyWindow}, late window is {_lateWindow}");

		// handle the tutorial indicator if needed
		if (_tutorialMode)
		{
			// if the tutorial indicator does not exist and it should, create it
			if (_tutorialIndicator == null)
			{
				_tutorialIndicator = Instantiate(_tutorialIndicatorPrefab, transform); // create indicator as child of this enemy
				_tutorialIndicator.transform.localPosition = new Vector3(0, 3.5f, 0);
			}
			_tutorialIndicator.GetComponent<TutorialIndicator>().SetTimings(_earlyWindow, _lateWindow);
			_tutorialIndicator.SetActive(false); // deactivate until the attack is about to happen
		}

		// do a deep copy of the set beats, so the original set beats aren't destroyed for future enemies
		_beats = new List<EnemyBeat>();
		foreach (EnemyBeat beat in _setBeats)
		{
			EnemyBeat newBeat = new EnemyBeat();
			newBeat.BeatOffset = beat.BeatOffset;
			newBeat.Sound = beat.Sound;
			newBeat.MoveByBeat = beat.MoveByBeat;
			_beats.Add(newBeat);
		}

		if (_deathSFX != null) _deathSFX.dir = _direction;
		if (_hitSFX   != null) _hitSFX  .dir = _direction;
		if (_guideSFX != null) _guideSFX.dir = _direction;

		gameObject.SetActive(true);

		_currentBeat = sb;
		CheckBeatAction();

        _moveCoroutine = StartCoroutine(SimpleMove());
    }

	void SetAnimDirection()
	{
		if (_direction == StageDirection.FORWARD) // Configure for front
		{
			if (frontOverride != null)
			{
				_anim.runtimeAnimatorController = frontOverride;
			}
			else
			{
				_sprite.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			}
		}
		else // Configure for sides
        {
            _sprite.flipX = _direction == StageDirection.RIGHT;
            if (sideOverride != null)
            {
                _anim.runtimeAnimatorController = sideOverride;
			}
			else
			{
				_sprite.transform.rotation = Quaternion.Euler(0f, 0f, _direction == StageDirection.LEFT ? 35f : -35f);
			}
		}
	}

	private void OnEnable()
	{
		EventManager.EventSubscribe(EventType.BEAT, BeatHandler);
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		EventManager.EventUnsubscribe(EventType.BEAT, BeatHandler);
		EventManager.EventUnsubscribe(EventType.PARRY_INPUT, InputHandler);
	}

	// Handler for when Player has successfully hit enemy
	public void InputHandler(object data)
	{
		if (data == null) return;

		StageDirection dir = (StageDirection)data;

		if (dir != _direction) return;

		// print to console the timing window
		float ms = (float)((Conductor.RawSongBeat - (_hitTime + _startBeat)) * 1000.0 / Conductor.CurrentBPS);
		Debug.Log($"Enemy was hit! Timing was {Mathf.Abs(ms)}ms " + (ms < 0 ? "early" : "late"));

		if (_deathClip != null)
		{
			EventManager.EventTrigger(EventType.SFX, _deathSFX);
		}

		(StageDirection, double) pair = (dir, (Conductor.SongBeat - (_hitTime + _startBeat)) / Conductor.CurrentBPS);

		EventManager.EventTrigger(EventType.PARRY_HIT, pair);

		if(_spawnOnDeath != null)
		{
			if(_direction == StageDirection.FORWARD)
			{
				Instantiate(_spawnOnDeath, transform.TransformPoint(_forwardSpawnOffset), transform.rotation, null);
			}
			else
			{
				Vector3 offset = _rightSpawnOffset;
				if (_sprite.flipX) offset.x = -offset.x;

				Instantiate(_spawnOnDeath, transform.TransformPoint(offset), transform.rotation, null);
			}
		}

		gameObject.SetActive(false);
	}

	// Listens every half beat, updates currentBeat, and performs actions (movement, sound, etc.)
	public void BeatHandler(object data)
	{
		_currentBeat = (float)data;

		CheckBeatAction();
	}

	// Check if any enemy-specific actions need to occur on beat
	private void CheckBeatAction()
	{
		double relativeBeat = _currentBeat - _startBeat;
		float nearestHalfBeat = Mathf.RoundToInt((float)relativeBeat * 2) / 2; // look idk man
		List<EnemyBeat> deleteBeats = new List<EnemyBeat>();

		// check if any new beats/animations/movement needs to occur
		foreach (EnemyBeat b in _beats)
		{
			if (b.Sound == null) continue;

			if (nearestHalfBeat >= b.BeatOffset)
			{
				float beatMoveTime;

				if (_beats.Count > 1)
				{
					beatMoveTime = b.MoveByBeat;
				}
				// If final beat, trigger attack animation and account for hit window
				else
				{
					beatMoveTime = b.MoveByBeat + (_hitWindow * (float)Conductor.CurrentBPS);
					_anim.SetTrigger("IsAttacking");
				}

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

		// if attack is coming up, start running the enumerator
		if (_attackCoroutine == null && _currentBeat + 1 > _earlyWindow)
		{
            _attackCoroutine = StartCoroutine(CheckAttack());

			// if in tutorial, since the timing window's about to happen, show the tutorial indicator to give the player a warning
			if (_tutorialMode)
			{
				_tutorialIndicator.SetActive(true);
			}
		}
	}

	// Check if player can attack enemy or vice versa
	IEnumerator CheckAttack()
	{
		float currentBeat;

        // Wait until the enemy is about to attack
        while (_earlyWindow >= (currentBeat = Conductor.SongBeat)) yield return null;

        Debug.Log($"Ready to attack on beat {currentBeat}");

        EventManager.EventSubscribe(EventType.PARRY_INPUT, InputHandler);
        manager._awaitingInput[_direction]++;

		if (_tutorialMode && _guideSFX != null)
		{
			// Wait until the perfect timing in the tutorial to play a sound to help with cueing
			while ((_hitTime + _startBeat) >= (currentBeat = Conductor.SongBeat)) yield return null;

            EventManager.EventTrigger(EventType.SFX, _guideSFX);
        }

        // Wait until it is too late to hit the enemy
        while (_lateWindow >= (currentBeat = Conductor.SongBeat)) yield return null;

		// And then attack
        Debug.Log($"Enemy has dealt damage! Beat was ({Conductor.SongBeat}, {_startBeat}+{HitTime}->{_earlyWindow}|{_lateWindow})", this);

        EventManager.EventUnsubscribe(EventType.BEAT, BeatHandler);
        EventManager.EventUnsubscribe(EventType.PARRY_INPUT, InputHandler);
        manager._awaitingInput[_direction]--;

        // Deal damage to player
        EventManager.EventTrigger(EventType.PLAYER_HIT, _direction);
        if (_hitSFX != null)
        {
            EventManager.EventTrigger(EventType.SFX, _hitSFX);
        }

        StartCoroutine(FadeOut());

        _attackCoroutine = null;

        yield break;
    }

	/*IEnumerator MoveOnBeat(float moveByBeat)
	{
		float distLeft = _moveIntervalDist * (_beats.Count - 1);
		float moveIntervalTime = moveByBeat * Conductor.SecondsPerBeat; // How long in seconds an Enemy has to move (based off of how many seconds a beat is)
		float moveStartBeat = _currentBeat;

		_accelCurve.ClearKeys();

		Keyframe startKey = new Keyframe(0, 0);
		startKey.weightedMode = WeightedMode.Both;
		startKey.outWeight = 0.5f;
		Keyframe middleKey = new Keyframe(moveIntervalTime * 0.33f, 0); // Appears to wait for a third of the move interval time
		middleKey.weightedMode = WeightedMode.Both;
		middleKey.inWeight = 0.5f;
		middleKey.outWeight = 0.5f;
		Keyframe endKey = new Keyframe(moveIntervalTime, _maxAccel);
		endKey.weightedMode = WeightedMode.Both;
		endKey.outWeight = 0.5f;

		_accelCurve.AddKey(startKey);
		_accelCurve.AddKey(middleKey);
		_accelCurve.AddKey(endKey);

		while (Vector3.Distance(_endPosition, transform.position) > distLeft)
		{
			float velocity = _accelCurve.Evaluate(Conductor.RawSongBeat - moveStartBeat);
			transform.position = Vector3.MoveTowards(transform.position, _endPosition, 
				(velocity * Time.deltaTime));

			yield return null;
		}
	}*/

	private IEnumerator SimpleMove()
	{
		float lastMoveBeat = 0;
		float nextMoveBeat = _moveBeats.Length == 0 ? _lateWindow : _startBeat + _moveBeats[0].beat;
		float lastMovePercent = 0;
		float nextMovePercent = _moveBeats.Length == 0 ? 1 : _moveBeats[0].percentage;
		uint curMoveBeat = 0;

		while (Conductor.RawSongBeat < _lateWindow)
		{
			if(curMoveBeat < _moveBeats.Length)
			{
				while(Conductor.RawSongBeat > nextMoveBeat)
				{
					lastMoveBeat = nextMoveBeat;
					lastMovePercent = nextMovePercent;
					curMoveBeat++;

					if(curMoveBeat == _moveBeats.Length)
					{
						nextMoveBeat = _lateWindow;
						nextMovePercent = 1;
						break;
					}
					else
					{
						nextMoveBeat = _startBeat + _moveBeats[curMoveBeat].beat;
						nextMovePercent = _moveBeats[curMoveBeat].percentage;
					}
				}
			}

			transform.position = Vector3.LerpUnclamped
			(
				_startPosition,
				_endPosition,
				Mathf.Lerp
				(
					lastMovePercent,
					nextMovePercent,
					_movementAnimation.Evaluate
					(
						Mathf.InverseLerp
						(
							lastMoveBeat,
							nextMoveBeat,
							Conductor.RawSongBeat
						)
					)
				)
			);

			yield return null;
		}

		_moveCoroutine = null;

		yield break;
	}

	IEnumerator FadeOut()
	{
		if (_moveCoroutine != null)
		{
			StopCoroutine(_moveCoroutine);
			_moveCoroutine = null;
		}

		// Start fading out the enemy
		_anim.SetTrigger("IsFading");

		while (true)
		{
			transform.position = Vector3.MoveTowards(transform.position, _startPosition, 0.5f * Time.deltaTime);
			yield return null;
		}
	}

	public void DisableEnemy()
	{
		StopAllCoroutines();
		_moveCoroutine = null;
		_attackCoroutine = null;
		gameObject.SetActive(false);
	}

	public float HitTime
	{
		get { return _hitTime; }
	}
}
