using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private EventManager eventManager;
    private Button button;

    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text selectText;

    public Type CurrentEventType { get; private set; }

    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
        button = GetComponent<Button>();

        button.onClick.AddListener(OnClickCard);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickCard);
    }

    public void EnableButton()
    {
        button.interactable = true;
    }

    public void DisableButton()
    {
        button.interactable = false;
    }

    /// <summary>
    /// Assigns the given event type to the card and updates the card visuals.
    /// </summary>
    /// <param name="eventType">The type of the event to assign.</param>
    public void AssignCardEvent(Type eventType)
    {
        CurrentEventType = eventType;

        WorldEventSO worldEvent = eventManager.GetEvent(CurrentEventType);

        nameText.text = $"{worldEvent.EventName}";
        descriptionText.text = $"{worldEvent.Description}";
    }

    private void OnClickCard()
    {
        eventManager.ChangeEvent(CurrentEventType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectText.text = "<b>> SELECT <<b>";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selectText.text = "SELECT";
    }
}
