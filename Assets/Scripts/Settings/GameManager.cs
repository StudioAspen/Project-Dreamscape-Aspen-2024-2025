using DG.Tweening.Core.Easing;
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
    public GameState PreviousState { get; private set; }
    public Action<GameState> OnGameStateChanged = delegate { };

    #region Time Scale
    public float DefaultFixedDeltaTime { get; private set; }
    #endregion

    private void Awake()
    {
        DefaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Start()
    {
        ForceChangeState(GameState.EVENT_SELECTION);
    }

    private void Update()
    {
        UpdateState(CurrentState);
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
        PreviousState = CurrentState;

        switch (newState)
        {
            case GameState.PLAYING:
                Time.timeScale = 1f;
                break;
            case GameState.PAUSED:
                Time.timeScale = 0f;
                break;
            case GameState.BIOME_SELECTION:
                Time.timeScale = 0f;
                break;
            case GameState.LAND_PLACEMENT:
                Time.timeScale = 0f;
                break;
            case GameState.LAND_EMPOWERMENT:
                Time.timeScale = 0f;
                break;
            case GameState.EVENT_SELECTION:
                Time.timeScale = 0f;
                break;
            case GameState.ASPECT_SELECTION:
                Time.timeScale = 0f;
                break;
            default:
                break;
        }

        CurrentState = newState;

        OnGameStateChanged?.Invoke(newState);
    }

    public void ForceChangeState(GameState newState)
    {
        switch (newState)
        {
            case GameState.PLAYING:
                Time.timeScale = 1f;
                break;
            case GameState.PAUSED:
                Time.timeScale = 0f;
                break;
            case GameState.BIOME_SELECTION:
                Time.timeScale = 0f;
                break;
            case GameState.LAND_PLACEMENT:
                Time.timeScale = 0f;
                break;
            case GameState.LAND_EMPOWERMENT:
                Time.timeScale = 0f;
                break;
            case GameState.EVENT_SELECTION:
                Time.timeScale = 0f;
                break;
            default:
                break;
        }

        CurrentState = newState;

        OnGameStateChanged?.Invoke(newState);
    }
    #endregion
}