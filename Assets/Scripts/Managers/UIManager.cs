using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Panels
    [SerializeField] GameObject _healthPanel;

    // Health UI
    [SerializeField] GameObject _healthIconPrefab;
    List<GameObject> _objHealthIcons;
    List<HealthIcon> _healthIcons;

    [SerializeField] Player _player;

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.HEALTH_UI, UpdateHealthUIHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.HEALTH_UI, UpdateHealthUIHandler);
    }

	private void Awake()
    {
        SetupHealthUI();
    }

	#region HEALTHUI

	// Awake executes before OnEnable which would have caused order effects with
	// event subscription; instead, just get the players health directly.

	// Create pool of health icons based on player's max health count
	void SetupHealthUI()
    {
        _objHealthIcons = ObjectPooler.CreateObjectPool(_player.MaxHealth, _healthIconPrefab, _healthPanel.transform);
        
        foreach (GameObject obj in _objHealthIcons)
        {
            obj.SetActive(true);
        }

        CreateHealthIconList();
    }

    // Create list to access the HealthIcon script
    void CreateHealthIconList()
    {
        _healthIcons = new List<HealthIcon>();

        foreach (GameObject obj in _objHealthIcons)
        {
            _healthIcons.Add(obj.GetComponent<HealthIcon>());
        }
    }

    // Update Health UI based on player's health
    public void UpdateHealthUIHandler(object data)
    {
        uint player_health = (uint)data;
        //ResetHealthUI();
        uint icon_health = 0;

        foreach (HealthIcon icon in _healthIcons)
		{
			icon.IsFull = icon_health < player_health;
			icon_health++;
        }
    }

    // Have all health icons be depleated
    void ResetHealthUI()
    {
        foreach (HealthIcon icon in _healthIcons)
        {
            icon.IsFull = false;
        }
    }
    #endregion
}
