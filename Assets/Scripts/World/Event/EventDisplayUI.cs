using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventDisplayUI : MonoBehaviour
{
    private EventManager eventManager;
    private TMP_Text titleText;

    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
        titleText = GetComponentInChildren<TMP_Text>();

        eventManager.OnEventChanged += EventManager_OnEventChanged;
    }

    private void OnDestroy()
    {
        eventManager.OnEventChanged -= EventManager_OnEventChanged;
    }

    private void EventManager_OnEventChanged(WorldEventSO newEvent)
    {
        titleText.text = $"{newEvent.EventName} Event";
    }
}
