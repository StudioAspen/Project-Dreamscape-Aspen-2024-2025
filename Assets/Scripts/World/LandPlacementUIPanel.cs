using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LandPlacementUIPanel : MonoBehaviour
{
    private ProgressionManager progressionManager;
    private InputManager inputManager;

    [Header("References")]
    [SerializeField] private TMP_Text landPlacementText;

    private void Awake()
    {
        progressionManager = FindObjectOfType<ProgressionManager>();
        inputManager = FindObjectOfType<InputManager>(); 
    }

    private void Update()
    {
        HandleLandPlacementText();
    }

    private void HandleLandPlacementText()
    {
        string landPlacementKey = inputManager.CurrentControlScheme == InputManager.ControlScheme.GAMEPAD ? "A" : "M1";
        landPlacementText.text = $"{landPlacementKey} - Place Land";
    }
}
