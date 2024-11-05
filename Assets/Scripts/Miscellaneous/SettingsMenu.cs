using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    private GameObject SettingsCanvas;
    [SerializeField] private PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        SettingsCanvas = transform.Find("SettingsMenu")?.gameObject;

        if(SettingsCanvas != null)
        {
            SettingsCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("SETTINGS MENU NOT FOUND");
        }
    }

    public void ToggleMenu() {
        Debug.Log("MENU OPENED");
        if (SettingsCanvas != null)
        {
            // Open Settings Menu
            // Display Canvas, Freeze Time, Show Cursor, Disable Lookaround
            if (SettingsCanvas.activeSelf)
            {
                SettingsCanvas.SetActive(true);
                Time.timeScale = 0;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerInput.actions.FindActionMap("Gameplay").FindAction("CameraLook").Disable();
            }
            // Close Settings Menu
            // Hide Canvas, Resume Time, Hide Cursor, Enable Lookaround
            else
            {
                SettingsCanvas.SetActive(false);
                Time.timeScale = 1;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerInput.actions.FindActionMap("Gameplay").FindAction("CameraLook").Enable();
            }
        }
    }
}
