using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerInputManager : MonoBehaviour
{
    private GameManager gameManager;

    public PlayerControls PlayerControls { get; private set; }
    public string CurrentControlScheme { get; private set; } = "KeyboardMouse"; // Default

    private void Awake()
    {
        PlayerControls = new PlayerControls();

        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        PlayerControls.Enable();

        InputSystem.onAnyButtonPress.Call(InputSystem_OnAnyButtonPress);

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDisable()
    {
        PlayerControls.Disable();

        InputSystem.onAnyButtonPress.Call(null);

        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void InputSystem_OnAnyButtonPress(InputControl control)
    {
        // Detect the current control scheme
        if (control.device is Gamepad)
        {
            SetControlScheme("Gamepad");
        }
        else if (control.device is Keyboard || control.device is Mouse)
        {
            SetControlScheme("KeyboardMouse");
        }
    }

    /// <summary>
    /// Sets the control scheme for the player input manager.
    /// </summary>
    /// <param name="controlScheme">The name of the control scheme to set.</param>
    private void SetControlScheme(string controlScheme)
    {
        if (controlScheme != CurrentControlScheme)
        {
            CurrentControlScheme = controlScheme;
            Debug.Log($"Control scheme changed to: {controlScheme}");

            // Perform any additional logic needed when control scheme changes
        }
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        PlayerControls.Gameplay.Disable();
        PlayerControls.LandPlacement.Disable();
        PlayerControls.LandEmpowerment.Disable();

        switch (newState)
        {
            case GameState.PLAYING:
                PlayerControls.Gameplay.Enable();
                break;
            case GameState.PAUSED:
                break;
            case GameState.BIOME_SELECTION:
                break;
            case GameState.LAND_PLACEMENT:
                PlayerControls.LandPlacement.Enable();
                break;
            case GameState.LAND_EMPOWERMENT:
                PlayerControls.LandEmpowerment.Enable();
                break;
            case GameState.EVENT_SELECTION:
                break;
            case GameState.ASPECT_SELECTION:
                break;
            default:
                break;
        }
    }
}
