using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Bedroom/Item Config")]
public class BedroomItemConfigSO : ScriptableObject
{
    [field: Tooltip("MUST BE UNIQUE!")]
    [field: SerializeField] public int UniqueID { get; private set; } = 0;
    [field: Header("Display")]
    [field: SerializeField] public string DisplayName { get; private set; } = "Bedroom Item";
    [field: SerializeField, TextArea(5, 20)] public string Description { get; private set; } = "Description";

    [field: Header("Cost")]
    [field: SerializeField] public int Cost { get; private set; } = 0;

    [field:Header("Buff")]
    [field: SerializeField] public Stat ActivatedStatBuffType { get; private set; }
    [field: SerializeField] public float BuffMultiplier { get; private set; } = 1f;
    [field: SerializeField] public float BuffFlatIncrease { get; private set; } = 0f;

    public enum Stat
    {
        MAX_HEALTH,
        DEFENSE,
        SPEED,
        DAMAGE,
        ATTACK_SPEED,
    }
}
