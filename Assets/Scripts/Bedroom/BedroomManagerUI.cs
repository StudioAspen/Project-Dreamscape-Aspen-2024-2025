using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BedroomManagerUI : MonoBehaviour
{
    [SerializeField] private BedroomManager bedroomManager;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button purchaseButton;

    private void Start()
    {
        UpdatePanelContents();
    }

    private void UpdatePanelContents()
    {
        if(bedroomManager == null || bedroomManager.SelectedItem == null)
        {
            Debug.LogError("BedroomManager or SelectedItem is not set.");
            return;
        }

        costText.text = $"Cost: {bedroomManager.SelectedItem.Config.Cost}";
        displayNameText.text = $"{bedroomManager.SelectedItem.Config.DisplayName}";
        descriptionText.text = $"{bedroomManager.SelectedItem.Config.Description}";

        purchaseButton.interactable = !bedroomManager.SelectedItem.IsActivated;
    }

    public void OnNextItemButtonClicked()
    {
        bedroomManager.SelectNextItem();
        UpdatePanelContents();
    }

    public void OnPreviousItemButtonClicked()
    {
        bedroomManager.SelectPreviousItem();
        UpdatePanelContents();
    }

    public void OnPurchaseButtonClicked()
    {
        bedroomManager.TryPurchaseSelectedItem();
        UpdatePanelContents();
    }
}
