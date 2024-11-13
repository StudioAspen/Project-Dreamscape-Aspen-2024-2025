using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

public class SettingsMenu : MonoBehaviour
{
    private GameObject settingsCanvas;
    private GameObject pauseCanvas;
    private InputAction cameraLook;
    [SerializeField, Scene] GameManager gameManager;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] Toggle vsyncToggle;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        pauseCanvas = transform.Find("PauseUI")?.gameObject;
        settingsCanvas = transform.Find("SettingsUI")?.gameObject;
        cameraLook = playerInput.actions.FindActionMap("Gameplay").FindAction("CameraLook");

        if (settingsCanvas != null)
        {
            settingsCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("Settings Menu Canvas not Found");
        }
    }

    void Start()
    {
        // Load Fullscreen
        if (!PlayerPrefs.HasKey("fullscreenOn")) {
            PlayerPrefs.SetInt("fullscreenOn", 0);
        } else {
            if(PlayerPrefs.GetInt("fullscreenOn") == 1)
            {
                fullscreenToggle.isOn = true;
                ToggleFullscreen();
            }
        }

        // Load VSync
        if (!PlayerPrefs.HasKey("vsyncOn")) {
            PlayerPrefs.SetInt("vsyncOn", 0);
        } else {
            if (PlayerPrefs.GetInt("vsyncOn") == 1)
            {
                vsyncToggle.isOn = true;
                ToggleVsync();
            }
        }
    }

    public void ToggleMenu() {
        if (settingsCanvas != null)
        {
            // Open Settings Menu
            if (!settingsCanvas.activeSelf)
            {
                gameManager.ChangeState(GameState.PAUSED);
                pauseCanvas.SetActive(false);
                settingsCanvas.SetActive(true);
                Debug.Log("Settings Menu Opened");
            }
            // Close Settings Menu
            else
            {
                settingsCanvas.SetActive(false);
                Debug.Log("Settings Menu Closed");
                gameManager.ChangeState(GameState.PLAYING);
            }
        }
    }

    public void ToggleFullscreen()
    {
        if (fullscreenToggle.isOn)
        {
            Screen.fullScreen = true;
            Debug.Log("Fullscreen Enabled");
        }
        else
        {
            Screen.fullScreen = false;
            Debug.Log("Fullscreen Disabled");
        }
    }

    public void ToggleVsync()
    {
        if(vsyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
            Debug.Log("Vsync Enabled");
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            Debug.Log("Vsync Disabled");
        }
    }
}
