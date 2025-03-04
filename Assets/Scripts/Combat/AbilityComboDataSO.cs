using UnityEngine;
using Dreamscape.Abilities;

[CreateAssetMenu(fileName = "AbilityCombo", menuName = "Combo/Ability", order = 1)]
public class AbilityComboDataSO : ComboDataSO
{
    [field: Header("Ability Options")]
    [field: SerializeField] public CastedAbility AbilityPrefab { get; private set; }
}
