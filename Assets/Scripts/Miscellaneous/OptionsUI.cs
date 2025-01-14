using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OptionsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PauseButtonUI closeButton;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;

    private void Awake()
    {
        closeButton.OnButtonClicked += CloseButton_OnButtonClicked;

        QualitySettings.vSyncCount = 0;
        //cameraLook = playerInput.actions.FindActionMap("Gameplay").FindAction("CameraLook");
    }

    private void OnDestroy()
    {
        closeButton.OnButtonClicked -= CloseButton_OnButtonClicked;
    }

    void Start()
    {
        /*// Load Fullscreen
        if (!PlayerPrefs.HasKey("fullscreenOn"))
        {
            PlayerPrefs.SetInt("fullscreenOn", 0);
        }
        else
        {
            if (PlayerPrefs.GetInt("fullscreenOn") == 1)
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
        }*/
    }

    private void CloseButton_OnButtonClicked()
    {
        gameObject.SetActive(false);
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
