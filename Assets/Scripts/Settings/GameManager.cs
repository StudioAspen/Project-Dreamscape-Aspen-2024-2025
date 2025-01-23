using DG.Tweening;
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
    private float previousTimeScale = 1f;
    #endregion

    private void Awake()
    {
        DefaultFixedDeltaTime = Time.fixedDeltaTime;
        Debug.Log(DefaultFixedDeltaTime);
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

        ForceChangeState(newState);
    }

    public void ForceChangeState(GameState newState)
    {
        PreviousState = CurrentState;

        switch (newState)
        {
            case GameState.PLAYING:
                SetTimeScale(previousTimeScale);
                break;
            case GameState.PAUSED:
                SetTimeScale(0, true);
                break;
            case GameState.BIOME_SELECTION:
                SetTimeScale(0);
                break;
            case GameState.LAND_PLACEMENT:
                SetTimeScale(0);
                break;
            case GameState.LAND_EMPOWERMENT:
                SetTimeScale(0);
                break;
            case GameState.EVENT_SELECTION:
                SetTimeScale(0);
                break;
            case GameState.ASPECT_SELECTION:
                SetTimeScale(0);
                break;
            default:
                break;
        }

        CurrentState = newState;

        OnGameStateChanged?.Invoke(newState);
    }
    #endregion

    #region Time Scale Functions
    /// <summary>
    /// Sets the time scale of the game.
    /// You can specify whether to save the previous time scale value.
    /// </summary>
    /// <param name="timeScale">The new time scale value.</param>
    /// <param name="trackPrevious">Whether to save the previous time scale value.</param>
    public void SetTimeScale(float timeScale, bool trackPrevious = false)
    {
        if (trackPrevious) previousTimeScale = Time.timeScale;
        else previousTimeScale = 1f;
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = DefaultFixedDeltaTime * timeScale;
    }
    #endregion
}