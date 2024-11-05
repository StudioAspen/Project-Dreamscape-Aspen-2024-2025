using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private GameObject PauseCanvas;
    private InputAction cameraLook;
    [SerializeField] private PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        PauseCanvas = transform.Find("PauseMenu")?.gameObject;
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

    public void TogglePauseMenu()
    {
        if (PauseCanvas != null)
        {
            // Open Pause Menu
            if(!PauseCanvas.activeSelf)
            {
                PauseCanvas.SetActive(true);
                Time.timeScale = 0;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cameraLook.Disable();
                Debug.Log("Pause Menu Opened");
            }
            else
            {
                PauseCanvas.SetActive(false);
                Time.timeScale = 1;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cameraLook.Enable();
                Debug.Log("Pause Menu Closed");
            }
        }
    }

}
