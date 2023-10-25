using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public EnemyManager manager;

	[SerializeField] private float _startBeat;
	// private List<Animation> _enemyAnims; // this may not work when changing directions? not needed for now
	[SerializeField] private List<EnemyBeat> _setBeats; // the defined list of beats this enemy type has
	private List<EnemyBeat> _beats; // the current list of beats of this specific enemy

	[SerializeField] private float _hitTime; // how many beats until this enemy attacks
	[SerializeField] private AudioClip _hitClip; // the sound this enemy plays if it attacks the player
	[SerializeField] private AudioClip _deathClip; // the sound this enemy plays if it is killed
	private float _hitWindow = 0.098f; // seconds leniency in both directions (consistent across all enemies)
	private float _earlyWindow; // how many beats the player can be early
	private float _lateWindow; // how many beats the player can be late
	[SerializeField] private StageDirection _direction;
	[SerializeField] private bool _attackReadied = false; // set to true when the event to kill this enemy is added
	private bool _checkingForAttack = false;
	private bool _hasAttacked = false;

	// Tutorial
	private bool _tutorialMode = false; // when true, show a red flash for the correct timing
	[SerializeField] private GameObject _tutorialIndicatorPrefab; // the object to spawn to show timings during the tutorial
	private GameObject _tutorialIndicator; // where the instance of the tutorial indicator is stored
	[SerializeField] private AudioClip _guideClip; // the sound that plays at the perfect hit timing
	private bool _hasPlayedGuide = false; // whether the perfect hit timing has occurred yet

	[SerializeField]
	private GameObject _spawnOnDeath = null;
	[SerializeField]
	public Vector3 _rightSpawnOffset = Vector3.zero;
	[SerializeField]
	public Vector3 _forwardSpawnOffset = Vector3.zero;

	// Data
	private SFXData _deathSFX;
	private SFXData _hitSFX;

	// Components
	[SerializeField]
	private SpriteRenderer _sprite;
	private Animator _anim;
	[SerializeField] private List<AnimatorOverrideController> _overrideControllers;

	// Beat Tracking
	private float _currentBeat;

	// Accel
	[SerializeField] private AnimationCurve _accelCurve;
	[SerializeField] private float _maxAccel;

	// Positioning
	private Vector3 _startPosition;
	private Vector3 _endPosition;
	private float _travelDistance;
    private float _moveIntervalDist; // How far in units an Enemy has to move

    private void Awake()
	{
		_deathSFX = new SFXData(_deathClip, StageDirection.FORWARD);
		_hitSFX = new SFXData(_hitClip, StageDirection.FORWARD);
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
		_travelDistance = Vector3.Distance(_startPosition, _endPosition);
        _moveIntervalDist = _travelDistance / _setBeats.Count;

        _attackReadied = false;
		_checkingForAttack = false;
		_hasAttacked = false;
		_hasPlayedGuide = false;

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

		_deathSFX.dir = _direction;
		_hitSFX.dir = _direction;

		gameObject.SetActive(true);

		_currentBeat = sb;
		CheckBeatAction();
	}

	void SetAnimDirection()
	{
		// Has overrides
		if (_overrideControllers.Count > 0)
		{
			// Set front override
			if (_direction == StageDirection.FORWARD)
			{
				_anim.runtimeAnimatorController = _overrideControllers[1];
			}
			// Set side override
			else
			{
				_anim.runtimeAnimatorController = _overrideControllers[0];

                if (_direction == StageDirection.LEFT && _sprite.flipX)
                {
                    _sprite.flipX = false;
                }
                else if (_direction == StageDirection.RIGHT && !_sprite.flipX)
                {
                    _sprite.flipX = true;
                }
            }
		}
		// Has no overrides (not direction-reliant)
		else
		{
            if (_direction == StageDirection.FORWARD)
            {
				_sprite.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            }
            else if (_direction == StageDirection.RIGHT)
            {
                _sprite.transform.rotation = Quaternion.Euler(0f, 0f, 145f);
            }
			else if (_direction == StageDirection.LEFT)
			{
                _sprite.transform.rotation = Quaternion.Euler(0f, 0f, 35f);
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
		if (_hasAttacked) return; // Do not process player hitting enemy if enemy has already attacked

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
				// Stop movement
				StopCoroutine("MoveOnBeat");

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

				// Start movement
                StartCoroutine(MoveOnBeat(beatMoveTime));

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
		if (!_checkingForAttack && _currentBeat + 1 > _earlyWindow)
		{
			StartCoroutine(CheckAttack());
			_checkingForAttack = true;

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
		while (!_hasAttacked)
		{
			float currentBeat = Conductor.SongBeat;
			// check if it's time to attack
			if (!_attackReadied && _earlyWindow < currentBeat)
			{
				_attackReadied = true;
				Debug.Log($"Ready to attack on beat {currentBeat}");
				EventManager.EventSubscribe(EventType.PARRY_INPUT, InputHandler);
				manager._awaitingInput[_direction]++;
			}

			// if past the perfect timing in the tutorial, play a sound to help with cueing
			if (_tutorialMode && !_hasPlayedGuide && currentBeat > (_hitTime + _startBeat))
			{
				_hasPlayedGuide = true;
				SFXData thisSound = new SFXData(_guideClip, _direction);
				EventManager.EventTrigger(EventType.SFX, thisSound);
			}

			// if the player hasn't destroyed this enemy in time, deal damage
			if (_attackReadied && currentBeat > _lateWindow)
			{
				Debug.Log($"Enemy has dealt damage! Beat was ({currentBeat}, {_startBeat}+{HitTime}->{_earlyWindow}|{_lateWindow})", this);

				_hasAttacked = true;
				manager._awaitingInput[_direction]--;

				// Deal damage to player
				EventManager.EventTrigger(EventType.PLAYER_HIT, _direction);
				if (_hitClip != null)
				{
					EventManager.EventTrigger(EventType.SFX, _hitSFX);
				}

                StartCoroutine(FadeOut());

                yield break;
			}

			yield return null;
		}
	}

	IEnumerator MoveOnBeat(float moveByBeat)
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
	}

	IEnumerator FadeOut()
	{
        StopCoroutine("MoveOnBeat");
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
        gameObject.SetActive(false);
    }

    public float HitTime
	{
		get { return _hitTime; }
	}
}
