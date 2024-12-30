using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private WorldManager worldManager;
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

        continueText.gameObject.SetActive(worldManager.EmpowerTokens + worldManager.WeakenTokens <= 0);
    }

    private void HandleTokensTexts()
    {
        empowerTokensText.text = $"M1 - Empower: {worldManager.EmpowerTokens}";
        weakenTokensText.text = $"M2 - Weaken: {worldManager.WeakenTokens}";

        empowerTokensText.color = worldManager.EmpowerTokens > 0 ? Color.green : Color.red;
        weakenTokensText.color = worldManager.WeakenTokens > 0 ? Color.green : Color.red;
    }
}