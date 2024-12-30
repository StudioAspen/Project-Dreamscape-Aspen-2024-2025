using KBCore.Refs;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventCardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private EventManager eventManager;
    [SerializeField, Self] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;

    public Type CurrentEventType { get; private set; }

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        button.onClick.AddListener(OnClickCard);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickCard);
    }

    private void Start()
    {
        
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
