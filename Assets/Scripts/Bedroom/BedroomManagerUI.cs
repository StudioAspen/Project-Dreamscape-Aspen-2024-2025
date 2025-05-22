using TMPro;
using UnityEngine;

public class BedroomManagerUI : MonoBehaviour
{
    [SerializeField] private BedroomManager bedroomManager;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private TMP_Text descriptionText;

    public void OnNextItemButtonClicked()
    {
        bedroomManager.SelectNextItem();
    }

    public void OnPreviousItemButtonClicked()
    {
        bedroomManager.SelectPreviousItem();
    }

    public void OnPurchaseButtonClicked()
    {
        bedroomManager.TryPurchaseSelectedItem();
    }
}
