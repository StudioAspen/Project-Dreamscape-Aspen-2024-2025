using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;


public class BedroomCameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera[] roomCameras; // The 10 POV cameras for the room.
    public CinemachineVirtualCamera overviewCamera; // The overview camera (optional).
    private CinemachineVirtualCamera activeCamera;
    private int currentCameraIndex = 0; // Starts at the first camera.

    public bool isInRoomEditMode = false; // Toggle for Room Edit Mode
    private bool isCycling = false; // Flag to control whether camera cycling is active

    // Reference to the main menu Canvas (for fading in and out)
    public Canvas mainMenuCanvas;
    public GameObject editRoomUI; // Edit Room UI

    public Button startButton; // Start button to be hidden in edit mode
    private Material[] uiMaterials; // Store the UI materials to preserve shaders

    private void Start()
    {
        // Assuming the mainMenuCanvas has Image components that use shaders
        // If there are multiple UI elements (e.g., buttons, background), get their materials
        Image[] uiImages = mainMenuCanvas.GetComponentsInChildren<Image>();
        uiMaterials = new Material[uiImages.Length];
        for (int i = 0; i < uiImages.Length; i++)
        {
            uiMaterials[i] = uiImages[i].material; // Store the materials with shaders
        }

        // Initially set the Overview Camera as the active camera
        activeCamera = overviewCamera;
        SetActiveCamera(activeCamera);
    }

    // Method to set the active camera with smooth transition
    private void SetActiveCamera(CinemachineVirtualCamera newCamera)
    {
        if (activeCamera != null)
            activeCamera.Priority = 0;

        activeCamera = newCamera;
        activeCamera.Priority = 10;

        // Optionally add a smooth transition effect (using DOTween)
        DOTween.To(() => activeCamera.m_Lens.FieldOfView,
                   x => activeCamera.m_Lens.FieldOfView = x,
                   45, 1f).SetEase(Ease.InOutSine);
    }

    // Method for cycling forward (right arrow or forward button)
    public void CycleForward()
    {
        if (!isInRoomEditMode) return; // Ignore cycling if not in Room Edit Mode

        currentCameraIndex++;
        if (currentCameraIndex >= roomCameras.Length)
        {
            // If we've reached the end of the loop, return to the overview camera
            currentCameraIndex = 0;
            SetActiveCamera(overviewCamera); // Switch to the overview camera
            DOVirtual.DelayedCall(1f, () =>
            {
                // After 1 second (for smooth transition), start cycling again
                SetActiveCamera(roomCameras[currentCameraIndex]);
            });
        }
        else
        {
            SetActiveCamera(roomCameras[currentCameraIndex]);
        }
    }

    // Method for cycling backward (left arrow or backward button)
    public void CycleBackward()
    {
        if (!isInRoomEditMode) return; // Ignore cycling if not in Room Edit Mode

        currentCameraIndex--;
        if (currentCameraIndex < 0)
        {
            // If we're at the first camera and cycle backward, go to the overview camera
            currentCameraIndex = roomCameras.Length - 1;
            SetActiveCamera(overviewCamera); // Switch to the overview camera
            DOVirtual.DelayedCall(1f, () =>
            {
                // After 1 second (for smooth transition), start cycling again
                SetActiveCamera(roomCameras[currentCameraIndex]);
            });
        }
        else
        {
            SetActiveCamera(roomCameras[currentCameraIndex]);
        }
    }

    // Call this method to toggle edit mode on and off
    public void ToggleRoomEditMode(bool isEditMode)
    {
        isInRoomEditMode = isEditMode;

        // Enable/Disable cycling based on edit mode
        if (isInRoomEditMode)
        {
            isCycling = true; // Allow camera cycling
        }
        else
        {
            isCycling = false; // Stop camera cycling
        }
    }

    // Method to return to the Overview Camera and fade back to the main menu
    public void ReturnToOverview()
    {
        // Stop camera cycling
        isCycling = false;

        // Switch to the overview camera
        SetActiveCamera(overviewCamera);

        // Hide the Start button when pressing Escape
        if (startButton != null)
        {
            startButton.gameObject.SetActive(false); // Hide the Start button
        }

        // Fade back in the main menu buttons and ensure shaders stay intact
        FadeInMainMenu();
    }

    // Method to fade in the main menu canvas (enable and fade in UI elements)
    private void FadeInMainMenu()
    {
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.gameObject.SetActive(true); // Enable Canvas

            // Fade in UI elements (change alpha from 0 to 1)
            CanvasGroup canvasGroup = mainMenuCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(1f, 1f).SetEase(Ease.InOutSine); // Fade in
            }

            // Restore the original materials/shaders for UI elements (if needed)
            Image[] uiImages = mainMenuCanvas.GetComponentsInChildren<Image>();
            for (int i = 0; i < uiImages.Length; i++)
            {
                uiImages[i].material = uiMaterials[i]; // Restore the material and shader
            }
        }
    }

    // Method to fade out the main menu canvas (disable UI elements)
    public void FadeOutMainMenu()
    {
        if (mainMenuCanvas != null)
        {
            // Fade out UI elements (change alpha from 1 to 0)
            CanvasGroup canvasGroup = mainMenuCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, 1f).SetEase(Ease.InOutSine); // Fade out
                DOVirtual.DelayedCall(1f, () => mainMenuCanvas.gameObject.SetActive(false)); // Optionally hide Canvas after fading out
            }
        }
    }

    private void Update()
    {

        // Detect keyboard input to cycle cameras (only if in edit mode)
        if (isInRoomEditMode && isCycling)
        {
            // Left arrow or A key to cycle backward
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                CycleBackward();
            }
            // Right arrow or D key to cycle forward
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                CycleForward();
            }
        }

        // Detect Escape key to return to Overview Camera and fade in Main Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToOverview();
        }
    }
}
