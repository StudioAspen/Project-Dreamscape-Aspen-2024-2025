using DG.Tweening;
using DG.Tweening.Core.Easing;
using Eflatun.SceneReference;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    PLAYING,
    PAUSED,
    BIOME_SELECTION,
    LAND_PLACEMENT,
    LAND_EMPOWERMENT,
    EVENT_SELECTION,
    ASPECT_SELECTION,
    GAME_OVER
}

public class GameManager : MonoBehaviour
{
    private PlayerControls playerControls;

    [Header("Scene")]
    [SerializeField] private SceneReference menuScene;

    public GameState CurrentState { get; private set; }
    public GameState PreviousState { get; private set; }
    public Action<GameState> OnGameStateChanged = delegate { };

    #region Time Scale
    public float DefaultFixedDeltaTime { get; private set; }
    private float previousTimeScale = 1f;
    #endregion

    private void Start()
    {
        DefaultFixedDeltaTime = Time.fixedDeltaTime;

        playerControls = FindObjectOfType<GameInputManager>().PlayerControls;

        playerControls.Gameplay.Pause.performed += PlayerControls_OnPausePerformed;

        ForceChangeState(GameState.PLAYING);
    }

    private void OnDestroy()
    {
        playerControls.Gameplay.Pause.performed -= PlayerControls_OnPausePerformed;
    }

    private void PlayerControls_OnPausePerformed(InputAction.CallbackContext context)
    {
        ChangeState(GameState.PAUSED);
    }

    #region State Machine Functions
    public void ChangeState(GameState newState)
    {
        if(CurrentState == newState) return;
        ForceChangeState(newState);
    }

    public void ForceChangeState(GameState newState)
    {
        Debug.Log($"Changing state to {newState}");
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
            case GameState.GAME_OVER:
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

    /// <summary>
    /// Resets the time scale to 1f so that in the next scene, the game runs at normal speed.
    /// Loads the menu scene.
    /// </summary>
    public void GoBackToMenu()
    {
        SetTimeScale(1f);
        SceneManager.LoadScene(menuScene.Name, LoadSceneMode.Single);
    }

    
}