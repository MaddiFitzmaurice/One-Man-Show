using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // NOTE: JUST A TEST FOR SFXMANAGER. Player sound will always
    // come from centre
    [SerializeField] [Range(1, 5)] int _maxHealth;
    [SerializeField] int _currentHealth;    // TEST, remove serialise when done

    // TO TEST HEALTH REGEN
    float _regenTimer;
    [SerializeField] float _timeToRegen;
    bool _isDead;

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

    private void Awake()
    {
        EventManager.EventInitialise(EventType.HEALTH_UI);
    }

    private void Start()
    {
        RegenHealth();
        EventManager.EventTrigger(EventType.HEALTH_UI, _currentHealth);
        _regenTimer = 0;
        _isDead = false;

        CreateSFXData();
    }

    private void Update()
    {
        // TEST, probably won't be doing this in update since it's poor form
        // TODO: Link this up with the beat manager instead of using update
        if (!_isDead)
        {
            _regenTimer += Time.deltaTime;

            if (_regenTimer > _timeToRegen)
            {
                RegenHealth();
            }
        }
    }

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

    public void PlayerHitHandler(object data)
    {
        // Subtract health and reset regen timer
        _currentHealth -= 1;
        _regenTimer = 0;

        // Play hit sfx
        SendSFXData(_hitSFXData);

        if (_currentHealth > -1)
        {
            EventManager.EventTrigger(EventType.HEALTH_UI, _currentHealth);
        }

        CheckHealthLeft();
    }

    // Check if player has died
    void CheckHealthLeft()
    {
        if (_currentHealth < 1)
        {
            // Signal end game Event
            Debug.Log("You lose");
        }
    }

    // Regen player's health to full
    void RegenHealth()
    {
        _currentHealth = _maxHealth;
        EventManager.EventTrigger(EventType.HEALTH_UI, _currentHealth);
    }

    // Create SFX data for player
    private void CreateSFXData()
    {
        // TEST, will get rid of later
        _SFXDataForward = new SFXData(_attackSFXForward, StageDirection.FORWARD);
        _SFXDataRight = new SFXData(_attackSFXRight, StageDirection.RIGHT);
        _SFXDataLeft = new SFXData(_attackSFXLeft, StageDirection.LEFT);
        _hitSFXData = new SFXData(_hitSFX, StageDirection.FORWARD);
    }

    // Send SFX data to play through the SFXManager
    private void SendSFXData(SFXData sfxData)
    {
        EventManager.EventTrigger(EventType.SFX, sfxData);
    }
}
