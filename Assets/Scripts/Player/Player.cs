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
			EventManager.EventTrigger(EventType.PLAYER_DIED, null);
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

	SFXData _playerHitSFXData;

	// Components
	SpriteRenderer _sprite;

	private void OnEnable()
	{
		EventManager.EventSubscribe(EventType.PARRY_HIT, ParryHitHandler);
		EventManager.EventSubscribe(EventType.PARRY_MISS, ParryMissHandler);
		EventManager.EventSubscribe(EventType.PLAYER_HIT, PlayerHitHandler);
		EventManager.EventSubscribe(EventType.BEAT, BeatHandler);
	}

	private void OnDisable()
	{
		EventManager.EventUnsubscribe(EventType.PARRY_HIT, ParryHitHandler);
		EventManager.EventUnsubscribe(EventType.PARRY_MISS, ParryMissHandler);
		EventManager.EventUnsubscribe(EventType.PLAYER_HIT, PlayerHitHandler);
		EventManager.EventUnsubscribe(EventType.BEAT, BeatHandler);
	}

	public void Awake()
	{
		_sprite = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		RegenHealth();
		CreateSFXData();
	}

	public void ParryHitHandler(object data)
	{
		if (data == null) return;

		StageDirection direction = (StageDirection)data;

		SFXData sfx = new SFXData(_parryHitSFX, direction);

		SendSFXData(sfx);

		switch (direction)
		{
			case StageDirection.FORWARD:
				Instantiate(_parryParticle, _particleParentForward);
				break;
			case StageDirection.LEFT:
				Instantiate(_parryParticle, _particleParentLeft);
				break;
			case StageDirection.RIGHT:
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
	}

	public void BeatHandler(object data)
	{
		_currentBeat = (float)data;

		CheckHealthRegen();
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

		CheckHealthLeft();
	}

	#region HEALTH
	void CheckHealthRegen()
	{
		if (IsDead) return;

		if (_currentBeat > _beatsTillRegen + _beatHitOn)
		{
			RegenHealth();
		}
	}

	// Check if player has died
	void CheckHealthLeft()
	{
		if (!IsDead) return;

		// Signal end game Event
		Debug.Log("You lose");
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
