using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class ProgressionUI : MonoBehaviour
{
    private GameManager gameManager;
    private ProgressionManager progressionManager;

    [Header("References")]
    [SerializeField] private TMP_Text empowerTokensText;
    [SerializeField] private TMP_Text weakenTokensText;
    [SerializeField] private TMP_Text continueText;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        progressionManager = FindObjectOfType<ProgressionManager>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.LAND_EMPOWERMENT)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
    }

    private void Update()
    {
        HandleTokensTexts();

        continueText.gameObject.SetActive(progressionManager.EmpowerTokens + progressionManager.WeakenTokens <= 0);
    }

    private void HandleTokensTexts()
    {
        empowerTokensText.text = $"M1 - Empower: {progressionManager.EmpowerTokens}";
        weakenTokensText.text = $"M2 - Weaken: {progressionManager.WeakenTokens}";

        empowerTokensText.color = progressionManager.EmpowerTokens > 0 ? Color.green : Color.red;
        weakenTokensText.color = progressionManager.WeakenTokens > 0 ? Color.green : Color.red;
    }
}
