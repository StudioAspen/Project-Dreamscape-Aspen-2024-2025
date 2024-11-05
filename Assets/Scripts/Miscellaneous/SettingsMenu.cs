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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.O)) {
            ToggleMenu();
        }
    }

    public void ToggleMenu() {
        Debug.Log("MENU OPENED");
        if (SettingsCanvas != null)
        {
            // Resume
            if (SettingsCanvas.activeSelf)
            {
                SettingsCanvas.SetActive(false);
                Time.timeScale = 1;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerInput.actions.FindActionMap("Gameplay").FindAction("CameraLook").Enable();
            }
            // Pause
            else
            {
                SettingsCanvas.SetActive(true);
                Time.timeScale = 0;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerInput.actions.FindActionMap("Gameplay").FindAction("CameraLook").Disable();
            }
        }
    }
}
