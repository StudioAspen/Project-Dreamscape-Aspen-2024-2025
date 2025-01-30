using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class WorldEventUI : MonoBehaviour
{
    private protected GameManager gameManager;
    private protected EventManager eventManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        eventManager = FindObjectOfType<EventManager>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        OnAwake();
    }

    private protected virtual void OnAwake()
    {

    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;

        OnOnDestroy();
    }

    private protected virtual void OnOnDestroy()
    {

    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.PLAYING)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Retrieves and validates the current event of the specified type.
    /// The return is garanteed to be non-null if the current state is of the specified existing type.
    /// If the current state is not of the specified type, this object will be destroyed.
    /// </summary>
    /// <typeparam name="WorldEventSOType">The type of the current event.</typeparam>
    /// <returns>The current event of the specified type, or null if not found or the current state is not of the specified type.</returns>
    private protected WorldEventSOType GetAndValidateCurrentEvent<WorldEventSOType>() where WorldEventSOType : WorldEventSO
    {
        WorldEventSOType currentEvent = eventManager.GetEvent<WorldEventSOType>();
        if (currentEvent == null)
        {
            Debug.LogError($"{typeof(WorldEventSOType)} not found in EventManager, destroying self");
            Destroy(gameObject);
            return null;
        }
        if (eventManager.CurrentEvent.GetType() != typeof(WorldEventSOType))
        {
            Debug.LogError($"Current state is not {typeof(WorldEventSOType)}, destroying self");
            Destroy(gameObject);
            return null;
        }

        return currentEvent;
    }
}
