using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    private GameInputManager gameInputManager;

    [Header("UI Panels")]
    [SerializeField, SerializedDictionary("Game State", "UI Panels")] private SerializedDictionary<GameState, UIPanel> gamePanels = new();

    // Call everything in start instead of Awake to allow initialization of UIPanels before disable
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameInputManager = FindObjectOfType<GameInputManager>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        gameInputManager.OnControlSchemeChanged += GameInputManager_OnControlSchemeChanged;

        // Init all the panels
        foreach(UIPanel panel in gamePanels.Values)
        {
            panel.Init(this, gameInputManager);
        }

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

    private void Update()
    {
        HandleNullSelected();
    }

    /// <summary>
    /// When the selected gameObject is null when in controller mode, tries to set the selected object to the default one
    /// </summary>
    private void HandleNullSelected()
    {
        if (gameInputManager.CurrentControlScheme == InputManager.ControlScheme.KEYBOARD_MOUSE) return;

        GameObject currentSelectedObject = EventSystem.current.currentSelectedGameObject;
        UIPanel currentPanel = gamePanels[gameManager.CurrentState];
        if (currentSelectedObject != null && currentSelectedObject.transform.IsChildOf(currentPanel.transform)) return;

        EventSystem.current.SetSelectedGameObject(currentPanel.DefaultSelectedObject);
    }
}
