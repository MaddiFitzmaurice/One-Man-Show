using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class Player : MonoBehaviour
{
	// Health
	[SerializeField] [Range(1, 5)] uint _maxHealth;
	[SerializeField] uint _currentHealth;    // TEST, remove serialise when done

	// Beat Data
	private float _currentBeat;
	private float _beatHitOn;
	[SerializeField] float _beatsTillRegen;
	[SerializeField] bool _regenEnabled = true;

	// Components
	Animator _anim;

	public uint MaxHealth { get => _maxHealth; }
	public uint CurrentHealth
	{
		get => _currentHealth;
		private set
		{
			if (value == _currentHealth) return;
			_currentHealth = value;
			EventManager.EventTrigger(EventType.HEALTH_UI, _currentHealth);

			if (value != 0) return;
			EventManager.EventUnsubscribe(EventType.BEAT, CheckHealthRegen);
			EventManager.EventTrigger(EventType.PLAYER_DIED, null);

			Debug.Log("Player died");
		}
	}
	public bool IsDead { get => _currentHealth == 0; }

	// Audio Data
	[SerializeField] AudioClip _attackSFXForward;
	[SerializeField] AudioClip _attackSFXLeft;
	[SerializeField] AudioClip _attackSFXRight;
	[SerializeField] AudioClip _playerHitSFX;
	[SerializeField] AudioClip _parryMissSFX;
	[SerializeField] AudioClip _parryHitSFX;

	[SerializeField] Transform _particleParentForward;
	[SerializeField] Transform _particleParentLeft;
	[SerializeField] Transform _particleParentRight;

	// Particles
	[SerializeField] private GameObject _parryParticle;
	[SerializeField] private GameObject _missParticle;

	SFXData _playerHitSFXData;

	// Components
	SpriteRenderer _sprite;

	private void OnEnable()
	{
		EventManager.EventSubscribe(EventType.PARRY_HIT, ParryHitHandler);
		EventManager.EventSubscribe(EventType.PARRY_MISS, ParryMissHandler);
		EventManager.EventSubscribe(EventType.PLAYER_HIT, PlayerHitHandler);
		EventManager.EventSubscribe(EventType.BEAT, BeatHandler);

		if (_regenEnabled)
		{
			EventManager.EventSubscribe(EventType.BEAT, CheckHealthRegen);
		}
	}

	private void OnDisable()
	{
		EventManager.EventUnsubscribe(EventType.PARRY_HIT, ParryHitHandler);
		EventManager.EventUnsubscribe(EventType.PARRY_MISS, ParryMissHandler);
		EventManager.EventUnsubscribe(EventType.PLAYER_HIT, PlayerHitHandler);
		EventManager.EventUnsubscribe(EventType.BEAT, BeatHandler);

		if (_regenEnabled && !IsDead)
		{
			EventManager.EventUnsubscribe(EventType.BEAT, CheckHealthRegen);
		}
	}

	public void Awake()
	{
		_sprite = GetComponent<SpriteRenderer>();
		_anim = GetComponent<Animator>();
	}

	private void Start()
	{
		RegenHealth();
		CreateSFXData();
	}

	public void ParryHitHandler(object data)
	{
		if (data == null) return;

		(StageDirection, double) pair = ((StageDirection, double))data;

		SFXData sfx = new SFXData(_parryHitSFX, pair.Item1);

		SendSFXData(sfx);

		switch (pair.Item1)
		{
			case StageDirection.FORWARD:
				_anim.SetTrigger("FrontAttack");
				Instantiate(_parryParticle, _particleParentForward);
				break;
			case StageDirection.LEFT:
                _anim.SetTrigger("LeftAttack");
                Instantiate(_parryParticle, _particleParentLeft);
				break;
			case StageDirection.RIGHT:
                _anim.SetTrigger("RightAttack");
                Instantiate(_parryParticle, _particleParentRight);
				break;
		}
	}

	public void ParryMissHandler(object data)
	{
		if (data == null) return;

		StageDirection direction = (StageDirection)data;

		SFXData sfx = new SFXData(_parryMissSFX, direction);

		SendSFXData(sfx);

		GameObject particle;
		switch (direction)
		{
			case StageDirection.FORWARD:
                _anim.SetTrigger("FrontAttack");
                particle = Instantiate(_missParticle, _particleParentForward);
				// rotate the particle by 90 degrees
				particle.transform.localRotation = Quaternion.Euler(0, 0, -90);
				break;
			case StageDirection.LEFT:
                _anim.SetTrigger("LeftAttack");
                particle = Instantiate(_missParticle, _particleParentLeft);
				// flip the particle horizontally
				Vector3 scale = particle.transform.localScale;
				particle.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
				break;
			case StageDirection.RIGHT:
                _anim.SetTrigger("RightAttack");
                Instantiate(_missParticle, _particleParentRight);
				break;
		}
	}

	public void BeatHandler(object data)
	{
		_currentBeat = (float)data;
	}

	public void PlayerHitHandler(object data)
	{
		// Already dead
		if (IsDead) return;

		// Subtract health and assign new beat hit on
		CurrentHealth -= 1;
		_beatHitOn = _currentBeat;

		// Play hit sfx
		SendSFXData(_playerHitSFXData);
	}

	#region HEALTH
	void CheckHealthRegen(object data)
	{
		if (IsDead) return;

		if (_currentBeat > _beatsTillRegen + _beatHitOn)
		{
			RegenHealth();
		}
	}

	// Regen player's health to full
	void RegenHealth()
	{
		CurrentHealth = _maxHealth;
	}
	#endregion

	#region SFX
	// Create SFX data for player
	private void CreateSFXData()
	{
		_playerHitSFXData = new SFXData(_playerHitSFX, StageDirection.FORWARD);
	}

	// Send SFX data to play through the SFXManager
	private void SendSFXData(SFXData sfxData)
	{
		EventManager.EventTrigger(EventType.SFX, sfxData);
	}
	#endregion

	public Vector2 GetSpriteSize()
	{
		return _sprite.bounds.size;
	}
}
