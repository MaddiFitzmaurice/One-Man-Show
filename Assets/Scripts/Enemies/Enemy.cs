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
	private SpriteRenderer _sprite;
	private Animator _anim;

	// Beat Tracking
	private float _currentBeat;

	[SerializeField]
	private AnimationCurve _movementAnimation;

	private Vector3 _startPosition;
	private Vector3 _endPosition;

	private void Awake()
	{
		_deathSFX = new SFXData(_deathClip, StageDirection.FORWARD);
		_hitSFX = new SFXData(_hitClip, StageDirection.FORWARD);
		_sprite = GetComponent<SpriteRenderer>();
		_anim = GetComponent<Animator>();
		_beats = new List<EnemyBeat>(); // just so it doesn't error out when the game starts
	}

	// set unique values for this enemy
	public void Initialise(StageDirection dr, float sb, Vector3 startPos, Vector3 endPos)
	{
		_startBeat = sb;
		_direction = dr;

		if (_direction == StageDirection.LEFT && _sprite.flipX)
		{
			_sprite.flipX = false;
		}
		else if (_direction == StageDirection.RIGHT && !_sprite.flipX)
		{
			_sprite.flipX = true;
		}

		_startPosition = startPos;
		_endPosition = endPos;

		_attackReadied = false;
		_checkingForAttack = false;

		transform.position = startPos;

		float beatDiff = _hitWindow * (float)Conductor.CurrentBPS; // what percentage of a beat the hit window falls within
		_earlyWindow = (_hitTime + _startBeat) - beatDiff;
		_lateWindow = (_hitTime + _startBeat) + beatDiff;
		Debug.Log($"Early window is beat {_earlyWindow}, late window is {_lateWindow}");

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
			_anim.SetTrigger("IsAttacking");
		}
	}

	// Check if player can attack enemy or vice versa
	IEnumerator CheckAttack()
	{
		// this object will get disabled eventually, so it's okay to have an infinite loop here
		while (true)
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
			// if the player hasn't destroyed this enemy in time, deal damage
			if (_attackReadied && currentBeat > _lateWindow)
			{
				Debug.Log($"Enemy has dealt damage! Beat was ({currentBeat}, {_startBeat}+{HitTime}->{_earlyWindow}|{_lateWindow})", this);

				manager._awaitingInput[_direction]--;

				// Deal damage to player
				EventManager.EventTrigger(EventType.PLAYER_HIT, _direction);
				if (_hitClip != null)
				{
					EventManager.EventTrigger(EventType.SFX, _hitSFX);
				}
				gameObject.SetActive(false);

				yield break;
			}

			yield return null;
		}
	}

	private void Update()
	{
		transform.position = Vector3.LerpUnclamped(
			_startPosition,
			_endPosition,
			Mathf.InverseLerp(
				_startBeat,
				_startBeat + _hitTime,
				(float)Conductor.RawLastBeat
				+ _movementAnimation.Evaluate(Conductor.RawSongBeat - Conductor.RawLastBeat)
			)
		);
	}

	public float HitTime
	{
		get { return _hitTime; }
	}
}
