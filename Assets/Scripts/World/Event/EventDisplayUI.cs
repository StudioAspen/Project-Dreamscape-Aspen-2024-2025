using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventDisplayUI : MonoBehaviour
{
    private GameManager gameManager;
    private EventManager eventManager;
    private TMP_Text titleText;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        eventManager = FindObjectOfType<EventManager>();
        titleText = GetComponentInChildren<TMP_Text>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Update()
    {
        if (eventManager.CurrentEvent == null) return;

        titleText.text = $"{eventManager.CurrentEvent.EventName} Event";
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.PLAYING)
        {
            Disable();
            return;
        }

        Enable();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
