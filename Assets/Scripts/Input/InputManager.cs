using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public class InputManager : MonoBehaviour
{
    private GameManager gameManager;

    public PlayerControls PlayerControls { get; private set; }

    public enum ControlScheme
    {
        KEYBOARD_MOUSE,
        GAMEPAD
    }
    public ControlScheme CurrentControlScheme { get; private set; } = ControlScheme.KEYBOARD_MOUSE;
    public Action<ControlScheme> OnControlSchemeChanged = delegate { };

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
            LockCursor();

            SetControlScheme(ControlScheme.GAMEPAD);
        }
        else if (device is Keyboard || device is Mouse)
        {
            if(!IsCurrentStateCursorLockedForKeyboardControl()) UnlockCursor();

            SetControlScheme(ControlScheme.KEYBOARD_MOUSE);
        }
    }

    /// <summary>
    /// Sets the control scheme variable.
    /// </summary>
    /// <param name="newControlScheme">The new control scheme to set.</param>
    private void SetControlScheme(ControlScheme newControlScheme)
    {
        if (newControlScheme != CurrentControlScheme)
        {
            CurrentControlScheme = newControlScheme;
            //Debug.Log($"Control Scheme Changed to: {CurrentControlScheme}");

            // Add your logic for switching input UI or behavior
            OnControlSchemeChanged?.Invoke(CurrentControlScheme);
        }
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        PlayerControls.Gameplay.Disable();
        PlayerControls.LandPlacement.Disable();
        PlayerControls.LandEmpowerment.Disable();
        PlayerControls.UI.Disable();

        LockCursor();

        switch (newState)
        {
            case GameState.PLAYING:
                PlayerControls.Gameplay.Enable();
                break;
            case GameState.PAUSED:
                PlayerControls.UI.Enable();
                UnlockCursor();
                break;
            case GameState.BIOME_SELECTION:
                PlayerControls.UI.Enable();
                UnlockCursor();
                break;
            case GameState.LAND_PLACEMENT:
                PlayerControls.LandPlacement.Enable();
                break;
            case GameState.LAND_EMPOWERMENT:
                PlayerControls.LandEmpowerment.Enable();
                break;
            case GameState.EVENT_SELECTION:
                PlayerControls.UI.Enable();
                UnlockCursor();
                break;
            case GameState.ASPECT_SELECTION:
                PlayerControls.UI.Enable();
                UnlockCursor();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Determines if the cursor should be locked for keyboard control in the current game state.
    /// </summary>
    /// <returns>True if the cursor should be locked, false otherwise.</returns>
    private bool IsCurrentStateCursorLockedForKeyboardControl()
    {
        return gameManager.CurrentState == GameState.PLAYING
            || gameManager.CurrentState == GameState.LAND_PLACEMENT
            || gameManager.CurrentState == GameState.LAND_EMPOWERMENT;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        if (CurrentControlScheme == ControlScheme.GAMEPAD) return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}


