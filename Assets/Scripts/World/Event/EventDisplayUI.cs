using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventDisplayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager;
    [SerializeField, Scene] private EventManager eventManager;
    [SerializeField, Child] private TMP_Text titleText;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        Disable();
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        Disable();
        if (newState != GameState.PLAYING)
        {
            return;
        }

        Enable();
    }

    public void Enable()
    {
        gameObject.SetActive(true);

        titleText.text = eventManager.CurrentEvent.GetType().Name;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
