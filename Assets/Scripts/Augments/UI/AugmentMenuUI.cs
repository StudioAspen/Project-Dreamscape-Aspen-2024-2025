using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AugmentMenuUI : MonoBehaviour
{
    [Header("Setters")]
    public GameObject augmentManager; // check for all augments possible
    public GameObject currentAugmentMenu; // current augments and levels
    public KeyCode openCurrentAugmentMenu; // button for opening menu

    [Header("Trackers")]
    private bool isOpen = false;

    /*
    private void Start()
    {
        // gets all augments possible for player to get
        var allAugments = augmentManager.GetComponentsInChildren<Augment>();

        for(int i = 0; i < allAugments.Length; i++)
        {
            Debug.Log("AUGMENT " + i + ": " + allAugments[i].ToString()); // prints all augments possible
            
            // prints augments if they are active
            if (allAugments[i].isActive)
                Debug.Log("AUGMENT active: " + allAugments[i].ToString());
        }
    }
    */

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
