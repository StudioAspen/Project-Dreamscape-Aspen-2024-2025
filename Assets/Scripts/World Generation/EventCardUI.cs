using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventCardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private WorldManager worldManager;
    [SerializeField, Scene] private EventManager eventManager;
    [SerializeField, Scene] private EventSelectUI eventSelectUI;
    [SerializeField, Self] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;
    private WorldEvent assignedWaveType;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        worldManager = FindObjectOfType<WorldManager>();
        eventSelectUI = FindObjectOfType<EventSelectUI>();
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
        //eventManager.CurrentWaveType = worldEvent;
        assignedWaveType = worldEvent;

        nameText.text = $"{worldEvent.ToString()}";
    }

    private void OnClickCard()
    {
        eventManager.AssignNextEvent(assignedWaveType);
        eventSelectUI.Disable();
    }
}
