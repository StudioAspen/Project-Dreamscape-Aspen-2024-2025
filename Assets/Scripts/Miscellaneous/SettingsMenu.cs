using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private GameObject SettingsCanvas;
    private GameObject PauseCanvas;
    private InputAction cameraLook;
    [SerializeField] GameManager gameManager;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] Toggle vsyncToggle;

    // Start is called before the first frame update
    void Start()
    {
        PauseCanvas = transform.Find("PauseMenu")?.gameObject;
        SettingsCanvas = transform.Find("SettingsMenu")?.gameObject;
        cameraLook = playerInput.actions.FindActionMap("Gameplay").FindAction("CameraLook");

        if (SettingsCanvas != null)
        {
            SettingsCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("Settings Menu Canvas not Found");
        }

        // Load Fullscreen
        if (!PlayerPrefs.HasKey("fullscreenOn"))
        {
            PlayerPrefs.SetInt("fullscreenOn", 0);
        }
        else
        {
            if(PlayerPrefs.GetInt("fullscreenOn") == 1)
            {
                fullscreenToggle.isOn = true;
                ToggleFullscreen();
            }
        }

        // Load VSync
        if (!PlayerPrefs.HasKey("vsyncOn"))
        {
            PlayerPrefs.SetInt("vsyncOn", 0);
        }
        else
        {
            if (PlayerPrefs.GetInt("vsyncOn") == 1)
            {
                vsyncToggle.isOn = true;
                ToggleVsync();
            }
        }
    }

    public void ToggleMenu() {
        if (SettingsCanvas != null)
        {
            // Open Settings Menu
            if (!SettingsCanvas.activeSelf)
            {
                gameManager.ChangeState(GameState.PAUSED);
                PauseCanvas.SetActive(false);
                SettingsCanvas.SetActive(true);
                Debug.Log("Settings Menu Opened");
            }
            // Close Settings Menu
            else
            {
                SettingsCanvas.SetActive(false);
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
