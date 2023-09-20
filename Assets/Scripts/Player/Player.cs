using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] [Range(1, 5)] uint _maxHealth;
    [SerializeField] uint _currentHealth;    // TEST, remove serialise when done

    private float _currentBeat;
    private float _beatHitOn;

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

	// TO TEST HEALTH REGEN
	//float _regenTimer;
    [SerializeField] float _timeToRegen;

    [SerializeField] AudioClip _attackSFXForward;
    [SerializeField] AudioClip _attackSFXLeft;
    [SerializeField] AudioClip _attackSFXRight;
    [SerializeField] AudioClip _hitSFX;
    SFXData _SFXDataForward;
    SFXData _SFXDataLeft;
    SFXData _SFXDataRight;
    SFXData _hitSFXData;

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PARRY_LEFT, ParryLeftHandler);
        EventManager.EventSubscribe(EventType.PARRY_RIGHT, ParryRightHandler);
        EventManager.EventSubscribe(EventType.PARRY_FORWARD, ParryForwardHandler);

        // TEST
        EventManager.EventSubscribe(EventType.PLAYER_HIT, PlayerHitHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PARRY_LEFT, ParryLeftHandler);
        EventManager.EventUnsubscribe(EventType.PARRY_RIGHT, ParryRightHandler);
        EventManager.EventUnsubscribe(EventType.PARRY_FORWARD, ParryForwardHandler);

        // TEST
        EventManager.EventUnsubscribe(EventType.PLAYER_HIT, PlayerHitHandler);
    }

    private void Start()
    {
        RegenHealth();
        CreateSFXData();
    }

    //private void Update()
    //{
    //    // TEST, probably won't be doing this in update since it's poor form
    //    // TODO: Link this up with the beat manager instead of using update
    //    if (IsDead) return;

    //    _regenTimer += Time.deltaTime;

    //    if (_regenTimer > _timeToRegen)
    //    {
    //        RegenHealth();
    //    }
    //}

    // Callback functions for Parry events that accept data
    public void ParryLeftHandler(object data)
    {
        Debug.Log("Test Parry Left");
        SendSFXData(_SFXDataLeft);
    }

    public void ParryRightHandler(object data)
    {
        Debug.Log("Test Parry Right");
        SendSFXData(_SFXDataRight);
    }

    public void ParryForwardHandler(object data)
    {
        Debug.Log("Test Parry Forward " + (int)data);
        SendSFXData(_SFXDataForward);
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

    void CheckHealthRegen()
    {
        if (IsDead) return;

        Debug.Log("hi");
        if (_currentBeat > _timeToRegen + _beatHitOn)
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
}
