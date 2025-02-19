using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuTransitions : MonoBehaviour
{
    public Canvas mainMenuCanvas; // Assign the main menu Canvas
    public GameObject editRoomUI; // Assign the Edit Room UI
    public Button startButton; // Assign the Start Button
    public Button[] menuButtons; // Assign all main menu buttons (Play, Settings, Credits, Quit)

    // Reference to CameraCycleManager
    public BedroomCameraManager cameraCycleManager;

    // This method is called when the Play button is clicked
    public void OnPlayButtonClicked()
    {
        // Fade out each button one by one
        foreach (Button btn in menuButtons)
        {
            btn.gameObject.GetComponent<CanvasRenderer>().SetAlpha(1f); // Ensure buttons are visible
            btn.gameObject.GetComponent<Graphic>().DOFade(0, 0.8f); // Fade out buttons
        }

        // Wait, then disable the menu & enable Edit Room UI
        DOVirtual.DelayedCall(0.8f, () =>
        {
            mainMenuCanvas.gameObject.SetActive(false); // Disable the main menu Canvas
            editRoomUI.SetActive(true); // Enable the Edit Room UI
            startButton.gameObject.SetActive(true); // Enable the Start Button
        });

        // Activate Room Edit Mode (camera cycling)
        if (cameraCycleManager != null)
        {
            cameraCycleManager.ToggleRoomEditMode(true); // Activate camera cycling in the edit room UI
            cameraCycleManager.FadeOutMainMenu(); // Optionally fade out the main menu
        }
    }
}
