using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using KBCore.Refs;

public class PauseMenu : MonoBehaviour
{
    private GameObject pauseUI;
    private InputAction cameraLook;
    private GameObject settingsUI;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField, Scene] GameManager gameManager;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        pauseUI = transform.Find("PauseUI")?.gameObject;
        settingsUI = transform.Find("SettingsUI")?.gameObject;
        //cameraLook = playerInput.actions.FindActionMap("Gameplay").FindAction("CameraLook");

        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
        else
        {
            Debug.Log("Pause Menu Canvas not Found");
        }
    }
    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Enable()
    {
        pauseUI.SetActive(true);
        //Debug.Log("Pause Menu Opened");
    }
    private void Disable()
    {
        pauseUI.SetActive(false);
        //Debug.Log("Pause Menu Closed");
    }


    private void GameManager_OnGameStateChanged(GameState newState)
    {
        // If game isn't paused, hide Pause Menu
        if (newState != GameState.PAUSED)
        {
            Disable();
            settingsUI.SetActive(false);
            return;
        }

        // On Pause -> Display Pause Menu
        Enable();
    }

    // Toggles Game State between Paused <-> Playing
    public void TogglePauseMenu()
    {
        if (gameManager.CurrentState == GameState.PAUSED)
        {
            gameManager.ChangeState(GameState.PLAYING);
        }
        else if (gameManager.CurrentState == GameState.PLAYING)
        {
            gameManager.ChangeState(GameState.PAUSED);
        }
    }

}
