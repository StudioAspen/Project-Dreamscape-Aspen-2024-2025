using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public class PlayerInputManager : MonoBehaviour
{
    private GameManager gameManager;

    public PlayerControls PlayerControls { get; private set; }

    public string CurrentControlScheme { get; private set; } = "Keyboard&Mouse";

    private void Awake()
    {
        PlayerControls = new PlayerControls();

        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        PlayerControls.Enable();

        InputSystem.onEvent += InputSystem_OnEvent;

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDisable()
    {
        PlayerControls.Disable();

        InputSystem.onEvent -= InputSystem_OnEvent;

        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void InputSystem_OnEvent(InputEventPtr eventPtr, InputDevice device)
    {
        // Detect the type of input source being used
        if (device is Gamepad)
        {
            SetControlScheme("Gamepad");
        }
        else if (device is Keyboard || device is Mouse)
        {
            SetControlScheme("Keyboard&Mouse");
        }
    }

    /// <summary>
    /// Sets the control scheme variable.
    /// </summary>
    /// <param name="newControlScheme">The new control scheme to set.</param>
    private void SetControlScheme(string newControlScheme)
    {
        if (newControlScheme != CurrentControlScheme)
        {
            CurrentControlScheme = newControlScheme;
            Debug.Log($"Control Scheme Changed to: {CurrentControlScheme}");

            // Add your logic for switching input UI or behavior
        }
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        PlayerControls.Gameplay.Disable();
        PlayerControls.LandPlacement.Disable();
        PlayerControls.LandEmpowerment.Disable();
        PlayerControls.UI.Disable();

        switch (newState)
        {
            case GameState.PLAYING:
                PlayerControls.Gameplay.Enable();
                break;
            case GameState.PAUSED:
                PlayerControls.UI.Enable();
                break;
            case GameState.BIOME_SELECTION:
                PlayerControls.UI.Enable();
                break;
            case GameState.LAND_PLACEMENT:
                PlayerControls.LandPlacement.Enable();
                break;
            case GameState.LAND_EMPOWERMENT:
                PlayerControls.LandEmpowerment.Enable();
                break;
            case GameState.EVENT_SELECTION:
                PlayerControls.UI.Enable();
                break;
            case GameState.ASPECT_SELECTION:
                PlayerControls.UI.Enable();
                break;
            default:
                break;
        }
    }
}
