using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Events Config", menuName = "Configs/Events Config")]
public class EventsConfigSO : ScriptableObject
{
    [field: Header("Survival Event")]
    [field: SerializeField] public float SurvivalEventDuration { get; private set; } = 60f;
    [field: SerializeField] public TMP_Text SurvivalEventUIPrefab { get; private set; }

    [field: Header("Zones Event")]
    [field: SerializeField] public int ZonesEventDummyVariable { get; private set; }

    [field: Header("Priorities Event")]
    [field: SerializeField] public int PrioritiesEventDummyVariable { get; private set; }

    [field: Header("Visit All Event")]
    [field: SerializeField] public int VisitAllEventDummyVariable { get; private set; }

    [field: Header("Defend Event")]
    [field: SerializeField] public float DefendEventDuration { get; private set; } = 60f;
    [field: SerializeField] public int DefendEventMaxHealth { get; private set; } = 200;
    [field: SerializeField] public DefendEventEntity DefendEventEntityPrefab { get; private set; }
    [field: SerializeField] public TMP_Text DefendEventUIPrefab { get; private set; }

    [field: Header("Escort Event")]
    [field: SerializeField] public float EscortEventDuration { get; private set; } = 60f;
    [field: SerializeField] public int EscortEventMaxHealth { get; private set; } = 200;
    [field: SerializeField] public EscortEventEntity EscortEventEntityPrefab { get; private set; }
    [field: SerializeField] public TMP_Text EscortEventUIPrefab { get; private set; }
}