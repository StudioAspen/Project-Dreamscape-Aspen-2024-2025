using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AugmentLevelController : MonoBehaviour
{
    [Header("Setters")]
    public GameObject augmentManager; // check for all augments possible
    public TextMeshProUGUI augmentsListText; // textbox

    public Augment[] allAugments;

    void Start()
    {
        // gets all augments possible for player to get
        allAugments = augmentManager.GetComponentsInChildren<Augment>();

        for (int i = 0; i < allAugments.Length; i++)
        {
            // prints all possible augments
            augmentsListText.text += allAugments[i].ToString() + " \n";

            // prints augments if they are active
            if (allAugments[i].isActive)
                Debug.Log("AUGMENT active: " + allAugments[i].ToString());
        }
    }

    private void Update()
    {

    }
}
