using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class ProgressionUIPanel : UIPanel
{
    private ProgressionManager progressionManager;

    [Header("References")]
    [SerializeField] private TMP_Text empowerTokensText;
    [SerializeField] private TMP_Text weakenTokensText;
    [SerializeField] private TMP_Text continueText;
    [SerializeField] private TMP_Text resetText;

    // UI scene loads last so Awake is safe here
    private void Awake()
    {
        progressionManager = FindObjectOfType<ProgressionManager>();
    }

    private void Update()
    {
        HandleTokensTexts();
        HandleContinueText();
        HandleResetText();

        continueText.gameObject.SetActive(progressionManager.EmpowerTokens + progressionManager.WeakenTokens <= 0);
    }

    private void HandleTokensTexts()
    {
        string empowerKey = gameInputManager.CurrentControlScheme == InputManager.ControlScheme.GAMEPAD ? "A" : "M1";
        string weakenKey = gameInputManager.CurrentControlScheme == InputManager.ControlScheme.GAMEPAD ? "B" : "M2";

        empowerTokensText.text = $"{empowerKey} - Empower: {progressionManager.EmpowerTokens}";
        weakenTokensText.text = $"{weakenKey} - Weaken: {progressionManager.WeakenTokens}";

        empowerTokensText.color = progressionManager.EmpowerTokens > 0 ? Color.green : Color.red;
        weakenTokensText.color = progressionManager.WeakenTokens > 0 ? Color.green : Color.red;
    }

    private void HandleContinueText()
    {
        if (progressionManager.CanProceedFromEmpowerment())
        {
            string continueKey = gameInputManager.CurrentControlScheme == InputManager.ControlScheme.GAMEPAD ? "Y" : "E";
            continueText.text = $"{continueKey} - Continue";
        }
    }

    private void HandleResetText()
    {
        string resetKey = gameInputManager.CurrentControlScheme == InputManager.ControlScheme.GAMEPAD ? "X" : "R";
        resetText.text = $"{resetKey} - Reset";
    }
}
