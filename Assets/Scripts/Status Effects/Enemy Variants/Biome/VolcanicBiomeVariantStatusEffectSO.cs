using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Volcanic Biome Variant Status Effect SO")]
public class VolcanicBiomeVariantStatusEffectSO : BiomeVariantStatusEffectSO
{
    // Base Class: You have access to the Entity entity being affected and the GameObject source of the object that applied this effect
    // TickStatusEffectSOClass: You have access to the int Ticks and float TicksDuration

    [field: Header("TickTemplate: Settings")]
    [field: SerializeField] public int TemplateInt { get; private set; } = 1;

    private protected override void OnApply()
    {
        base.OnApply();

        // your logic for when the effect is applied
    }

    private protected override void OnExpire()
    {
        base.OnExpire();

        // your logic for when the effect expires
    }

    public override void Cancel()
    {
        base.Cancel();

        // your logic for when the effect is cancelled
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        base.OnStack(newStatusEffect);

        DurationStatusEffectSO overridingStatusEffect = newStatusEffect as DurationStatusEffectSO;

        // your logic for when the effect is stacked
    }
}
