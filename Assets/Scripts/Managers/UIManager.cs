using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] List<HealthIcon> _healthIcons;

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.HEALTH_UI, UpdateHealthUIHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.HEALTH_UI, UpdateHealthUIHandler);
    }

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

            if (!icon.GetActive())
            {
                icon.SetActive(true);
                i++;
            }
        }
    }

    void ResetHealthUI()
    {
        foreach (HealthIcon icon in _healthIcons)
        {
            icon.SetActive(false);
        }
    }
}
