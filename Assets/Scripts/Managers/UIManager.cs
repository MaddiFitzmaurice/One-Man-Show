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
    void SetupHealthUI()
    {
        EventManager.EventUnsubscribe(EventType.HEALTH_UI, UpdateHealthUIHandler);
        EventManager.EventSubscribe(EventType.HEALTH_UI, SetupHealthUIHandler);
    }

    // Create pool of health icons based on player's max health count
    public void SetupHealthUIHandler(object data)
    {
        int num = (int)data;
        _objHealthIcons = ObjectPooler.CreateObjectPool(num, _healthIconPrefab, _healthPanel.transform);
        
        foreach (GameObject obj in _objHealthIcons)
        {
            obj.SetActive(true);
        }

        EventManager.EventUnsubscribe(EventType.HEALTH_UI, SetupHealthUIHandler);
        EventManager.EventSubscribe(EventType.HEALTH_UI, UpdateHealthUIHandler);

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
        int health = (int)data;
        ResetHealthUI();
        int i = 0;

        foreach (HealthIcon icon in _healthIcons)
        {
            if (i == health)
            {
                return;
            }

            if (!icon.GetIsFull())
            {
                icon.SetIsFull(true);
                i++;
            }
        }
    }

    // Have all health icons be depleated
    void ResetHealthUI()
    {
        foreach (HealthIcon icon in _healthIcons)
        {
            icon.SetIsFull(false);
        }
    }
    #endregion
}
