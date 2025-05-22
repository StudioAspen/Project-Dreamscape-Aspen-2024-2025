using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Bedroom Item Config")]
public class BedroomItemConfig : ScriptableObject
{
    [field: Header("Display")]
    [field: SerializeField] public string DisplayName { get; private set; } = "Bedroom Item";
    [field: SerializeField, TextArea(5, 20)] public string Description { get; private set; } = "Description";

    [field: Header("Config")]
    [field: SerializeField] public int Cost { get; private set; } = 0;
}