using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AugmentMenu : MonoBehaviour
{
    [Header("Setters")]
    public GameObject currentAugmentMenu; // current augments and levels
    public KeyCode openCurrentAugmentMenu; // button for opening menu

    [Header("Trackers")]
    private bool isOpen = false;

    public void Update()
    {
        // turns on/off menu
        if (Input.GetKeyDown(openCurrentAugmentMenu))
            isOpen = !isOpen;

        if (isOpen)
        {
            PauseGame();
            currentAugmentMenu.SetActive(true);
        }

        else
        {
            ResumeGame();
            currentAugmentMenu.SetActive(false);
        }
    }

    public void PauseGame() {
        Debug.Log("Paused");
        Time.timeScale = 0;
    }

    public void ResumeGame() {
        Debug.Log("Resumed");
        Time.timeScale = 1;
    }
}
