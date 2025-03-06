using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LandPlacementUIPanel : UIPanel
{
    [Header("References")]
    [SerializeField] private TMP_Text landPlacementText;

    private void Awake()
    {
    }

    private void Update()
    {
        HandleLandPlacementText();
    }

    private void HandleLandPlacementText()
    {
        string landPlacementKey = gameInputManager.CurrentControlScheme == InputManager.ControlScheme.GAMEPAD ? "A" : "M1";
        landPlacementText.text = $"{landPlacementKey} - Place Land";
    }
}
