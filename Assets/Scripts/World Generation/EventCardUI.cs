using KBCore.Refs;
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

    public WorldEvent CurrentEvent { get; private set; }

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();

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

    public void AssignCardEvent(WorldEvent worldEvent)
    {
        CurrentEvent = worldEvent;

        nameText.text = $"{CurrentEvent.ToString()}";
    }

    private void OnClickCard()
    {
        eventManager.AssignNextEvent(CurrentEvent);

        // Timer UI popup
        if (CurrentEvent == WorldEvent.SURVIVAL)
        {
            eventManager.EnableTimer();
            eventManager.ResetTimer();
        }
    }
}
