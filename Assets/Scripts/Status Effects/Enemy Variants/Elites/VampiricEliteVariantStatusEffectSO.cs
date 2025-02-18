using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Vampiric Elite Variant", menuName = "Status Effect/Enemy Variants/Elite/Vampiric")]
public class VampiricEliteVariantStatusEffectSO: EliteVariantStatusEffectSO
{
    [field: Header("Lifesteal")]
    [field: SerializeField] public LifeStealStatusEffectSO LifeStealStatusEffect { get; private set; }

    private protected override void OnApply()
    {
        base.OnApply();

        entityStatusEffectorOwner.ApplyStatusEffect(LifeStealStatusEffect, entity.gameObject);
    }

    public override void Cancel()
    {
        base.Cancel();

        entityStatusEffectorOwner.RemoveStatusEffect(LifeStealStatusEffect.GetType(), true);
    }
}
