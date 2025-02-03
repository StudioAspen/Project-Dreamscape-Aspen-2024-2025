using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    private GameInputManager gameInputManager;

    [Header("UI Panels")]
    [SerializeField, SerializedDictionary("Game State", "UI Panels")] private SerializedDictionary<GameState, UIPanel> gamePanels = new();

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameInputManager = FindObjectOfType<GameInputManager>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        gameInputManager.OnControlSchemeChanged += GameInputManager_OnControlSchemeChanged;

        // Manually call the event handlers to set up the initial state
        GameManager_OnGameStateChanged(gameManager.CurrentState);
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
        gameInputManager.OnControlSchemeChanged -= GameInputManager_OnControlSchemeChanged;
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        // Disable all panels first
        foreach(UIPanel panel in gamePanels.Values)
        {
            panel.gameObject.SetActive(false);
        }

        // Enable the panel for the new state
        UIPanel panelToActivate = gamePanels[newState];
        panelToActivate.gameObject.SetActive(true);
        panelToActivate.OnDeselected();
        if (gameInputManager.CurrentControlScheme == InputManager.ControlScheme.GAMEPAD) EventSystem.current.SetSelectedGameObject(panelToActivate.DefaultSelectedObject);
    }

    private void GameInputManager_OnControlSchemeChanged(InputManager.ControlScheme newControlScheme)
    {
        // Call the deselect method for the current panel
        UIPanel currentPanel = gamePanels[gameManager.CurrentState];
        currentPanel.OnDeselected();

        // Set the selected object for the new control scheme
        if (newControlScheme == InputManager.ControlScheme.GAMEPAD)
        {
            EventSystem.current.SetSelectedGameObject(currentPanel.DefaultSelectedObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
