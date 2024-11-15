using DG.Tweening.Core.Easing;
using KBCore.Refs;
using System;
using UnityEngine;

public enum GameState
{
    PLAYING,
    PAUSED,
    BIOME_SELECTION,
    LAND_PLACEMENT,
    LAND_EMPOWERMENT,
    EVENT_SELECTION,
    ASPECT_SELECTION
}

public class GameManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }
    public Action<GameState> OnGameStateChanged = delegate { };

    private void Awake()
    {
        
    }

    private void Start()
    {
        ChangeState(GameState.PLAYING);
    }

    private void Update()
    {
        UpdateState(CurrentState);

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (CurrentState == GameState.PLAYING) ChangeState(GameState.ASPECT_SELECTION);
            else if (CurrentState == GameState.ASPECT_SELECTION) ChangeState(GameState.PLAYING);
        }
    }

    #region State Machine Functions
    private void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.PLAYING:
                break;
            case GameState.PAUSED:
                break;
            case GameState.BIOME_SELECTION:
                break;
            case GameState.LAND_PLACEMENT:
                break;
            case GameState.LAND_EMPOWERMENT:
                break;
            case GameState.EVENT_SELECTION:
                break;
            case GameState.ASPECT_SELECTION:
                break;
            default:
                break;
        }
    }
    public void ChangeState(GameState newState)
    {
        if(CurrentState == newState) return;

        //print($"GameManager: Going from {CurrentState} to {newState}");

        switch (newState)
        {
            case GameState.PLAYING:
                Time.timeScale = 1f;

                LockCursor();
                break;
            case GameState.PAUSED:
                Time.timeScale = 0f;

                UnlockCursor();
                break;
            case GameState.BIOME_SELECTION:
                Time.timeScale = 0f;

                UnlockCursor();
                break;
            case GameState.LAND_PLACEMENT:
                Time.timeScale = 0f;

                LockCursor();
                break;
            case GameState.LAND_EMPOWERMENT:
                Time.timeScale = 0f;

                LockCursor();
                break;
            case GameState.EVENT_SELECTION:
                Time.timeScale = 0f;

                UnlockCursor();
                break;
            case GameState.ASPECT_SELECTION:
                Time.timeScale = 0f;

                UnlockCursor();
                break;
            default:
                break;
        }

        CurrentState = newState;

        OnGameStateChanged?.Invoke(newState);
    }
    #endregion  

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}