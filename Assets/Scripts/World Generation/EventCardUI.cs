using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventCardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private WorldManager worldManager;
    [SerializeField, Scene] private EventSelectUI eventSelectUI;
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
        worldManager = FindObjectOfType<WorldManager>();
        eventSelectUI = FindObjectOfType<EventSelectUI>();

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
        worldManager.AssignNextEvent(CurrentEvent);
        eventSelectUI.Disable();
    }
}
