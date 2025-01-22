using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Elite Variant Database", menuName = "World/Elite Variant Database")]
public class EliteVariantDatabaseSO : ScriptableObject
{
    [field: SerializeField] public List<EliteVariantStatusEffectSO> EliteVariantStatusEffects { get; private set; } = new List<EliteVariantStatusEffectSO>();
}