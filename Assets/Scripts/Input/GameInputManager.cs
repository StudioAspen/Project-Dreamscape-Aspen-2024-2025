using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Utilities;

public class GameInputManager : InputManager
{
    private GameManager gameManager;

    private protected override void OnAwake()
    {
        base.OnAwake();

        OnControlSchemeChanged += InputManager_OnControlSchemeChanged;
    }

    private protected override void OnStart()
    {
        base.OnStart();

        gameManager = FindObjectOfType<GameManager>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        GameManager_OnGameStateChanged(gameManager.CurrentState); // Manually call this to set the initial state. Race case handler.
        InputManager_OnControlSchemeChanged(CurrentControlScheme); // Race case handler.
    }

    private protected override void OnOnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;

        OnControlSchemeChanged -= InputManager_OnControlSchemeChanged;
    }

    private void InputManager_OnControlSchemeChanged(ControlScheme newControlScheme)
    {
        if(newControlScheme == ControlScheme.KEYBOARD_MOUSE)
        {
            if (!IsCurrentStateCursorLockedForKeyboardControl()) UnlockCursor();
        }
        else
        {
            LockCursor();
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
            case GameState.GAME_OVER:
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
}


