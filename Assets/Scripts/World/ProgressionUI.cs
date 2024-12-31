using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private ProgressionManager progressionManager;
    [SerializeField] private TMP_Text empowerTokensText;
    [SerializeField] private TMP_Text weakenTokensText;
    [SerializeField] private TMP_Text continueText;

    private void OnValidate()
    {
        this.ValidateRefs();
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