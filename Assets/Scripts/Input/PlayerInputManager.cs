using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance;

    private GameManager gameManager;

    public PlayerControls PlayerControls { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        if(PlayerControls == null)
        {
            PlayerControls = new PlayerControls();
        }

        PlayerControls.Enable();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDisable()
    {
        PlayerControls.Disable();

        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
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
