using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventCardUI : MonoBehaviour
{
    private EventManager eventManager;
    private Button button;

    [Header("References")]
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;

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

    public void AssignCardEvent(Type eventType)
    {
        CurrentEventType = eventType;

        nameText.text = $"{CurrentEventType.ToString()}";
    }

    private void OnClickCard()
    {
        eventManager.ChangeEvent(CurrentEventType);
    }
}
