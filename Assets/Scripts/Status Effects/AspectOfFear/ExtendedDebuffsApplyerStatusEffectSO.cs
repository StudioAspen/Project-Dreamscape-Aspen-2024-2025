using UnityEngine;

[CreateAssetMenu(fileName = "Extended Debuffs Applyer", menuName = "Status Effect/Aspect of Fear/Extended Debuffs Applyer")]
public class ExtendedDebuffsApplyerStatusEffectSO : StatusEffectSO
{
    [field: Header("Settings")]
    [field: SerializeField] public float ExtendedDebuffsMultiplier { get; private set; } = 1.25f;

    private protected override void OnApply()
    {
        base.OnApply();

        entity.DebuffApplyDurationMultiplier.AddMultiplier(ExtendedDebuffsMultiplier, this);
    }

    public override void Cancel()
    {
        base.Cancel();

        entity.DebuffApplyDurationMultiplier.ClearBuffsFromSource(this);
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        ExtendedDebuffsApplyerStatusEffectSO overridingStatusEffect = newStatusEffect as ExtendedDebuffsApplyerStatusEffectSO;

        ExtendedDebuffsMultiplier = overridingStatusEffect.ExtendedDebuffsMultiplier;
    }
}
