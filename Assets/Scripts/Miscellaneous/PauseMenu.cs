using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using KBCore.Refs;

public class PauseMenu : MonoBehaviour
{
    private GameObject PauseCanvas;
    private InputAction cameraLook;
    private GameObject SettingsCanvas;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField, Scene] private GameManager gameManager;

    private void Awake()
    {
        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }
    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            TogglePauseMenu();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        PauseCanvas = transform.Find("PauseMenu")?.gameObject;
        SettingsCanvas = transform.Find("SettingsMenu")?.gameObject;
        cameraLook = playerInput.actions.FindActionMap("Gameplay").FindAction("CameraLook");

        if (PauseCanvas != null)
        {
            PauseCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("Pause Menu Canvas not Found");
        }
    }


    private void Enable()
    {
        PauseCanvas.SetActive(true);
        Debug.Log("Pause Menu Opened");
    }

    private void Disable()
    {
        PauseCanvas.SetActive(false);
        Debug.Log("Pause Menu Closed");
    }


    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.PAUSED)
        {
            Disable();
            SettingsCanvas.SetActive(false);
            return;
        }

        Enable();
    }

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
