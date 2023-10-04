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
    [SerializeField] AudioClip _hitSFX;
    SFXData _SFXDataForward;
    SFXData _SFXDataLeft;
    SFXData _SFXDataRight;
    SFXData _hitSFXData;

    // Components
    SpriteRenderer _sprite;

    // Particles
    [SerializeField] private GameObject _parryParticle;

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PARRY_LEFT, ParryLeftHandler);
        EventManager.EventSubscribe(EventType.PARRY_RIGHT, ParryRightHandler);
        EventManager.EventSubscribe(EventType.PARRY_FORWARD, ParryForwardHandler);
        EventManager.EventSubscribe(EventType.PLAYER_HIT, PlayerHitHandler);
        EventManager.EventSubscribe(EventType.BEAT, BeatHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PARRY_LEFT, ParryLeftHandler);
        EventManager.EventUnsubscribe(EventType.PARRY_RIGHT, ParryRightHandler);
        EventManager.EventUnsubscribe(EventType.PARRY_FORWARD, ParryForwardHandler);
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

    // Callback functions for Parry events that accept data
    public void ParryLeftHandler(object data)
    {
        Debug.Log("Test Parry Left");
        SendSFXData(_SFXDataLeft);
        // spawn particle
        Vector3 particlePos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        GameObject particle = Instantiate(_parryParticle);
        particle.transform.position = particlePos;
    }

    public void ParryRightHandler(object data)
    {
        Debug.Log("Test Parry Right");
        SendSFXData(_SFXDataRight);
        // spawn particle
        Vector3 particlePos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        GameObject particle = Instantiate(_parryParticle);
        particle.transform.position = particlePos;
    }

    public void ParryForwardHandler(object data)
    {
        Debug.Log("Test Parry Forward " + (int)data);
        SendSFXData(_SFXDataForward);
        // spawn particle
        Vector3 particlePos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        GameObject particle = Instantiate(_parryParticle);
        particle.transform.position = particlePos;
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
        SendSFXData(_hitSFXData);

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
        // TEST, will get rid of later
        _SFXDataForward = new SFXData(_attackSFXForward, StageDirection.FORWARD);
        _SFXDataRight   = new SFXData(_attackSFXRight,   StageDirection.RIGHT  );
        _SFXDataLeft    = new SFXData(_attackSFXLeft,    StageDirection.LEFT   );
        _hitSFXData     = new SFXData(_hitSFX,           StageDirection.FORWARD);
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
