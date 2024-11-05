using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private GameObject SettingsCanvas;
    private InputAction cameraLook;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] Toggle vsyncToggle;

    // Start is called before the first frame update
    void Start()
    {
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
    }

    public void ToggleMenu() {
        if (SettingsCanvas != null)
        {
            // Open Settings Menu
            // Display Canvas, Freeze Time, Show Cursor, Disable Lookaround
            if (!SettingsCanvas.activeSelf)
            {
                SettingsCanvas.SetActive(true);
                Time.timeScale = 0;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cameraLook.Disable();
                Debug.Log("Settings Menu Opened");
            }
            // Close Settings Menu
            // Hide Canvas, Resume Time, Hide Cursor, Enable Lookaround
            else
            {
                SettingsCanvas.SetActive(false);
                Time.timeScale = 1;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cameraLook.Enable();
                Debug.Log("Settings Menu Closed");
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
