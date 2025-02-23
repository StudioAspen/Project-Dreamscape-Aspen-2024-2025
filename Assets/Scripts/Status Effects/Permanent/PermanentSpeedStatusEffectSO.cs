using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Permanent Speed")]
public class PermanentSpeedStatusEffectSO : StatusEffectSO
{
    [field: Header("Permanent Speed Status Effect: Settings")]
    [field: SerializeField] public float SpeedMultiplier { get; private set; } = 1.1f;

    private protected override void OnApply()
    {
        base.OnApply();

        entity.StatusSpeedModifier.ClearBuffsFromSource(this); // apply the new speed multiplier
    }

    public override void Cancel()
    {
        base.Cancel();

        entity.StatusSpeedModifier.ClearBuffsFromSource(this); // undo the speed multiplier
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        PermanentSpeedStatusEffectSO overridingStatusEffect = newStatusEffect as PermanentSpeedStatusEffectSO;

        entity.StatusSpeedModifier.ClearBuffsFromSource(this); // undo the speed multiplier
        SpeedMultiplier *= overridingStatusEffect.SpeedMultiplier; // update the speed multiplier
        entity.StatusSpeedModifier.AddMultiplier(SpeedMultiplier, this); // reapply the new updated speed multiplier
    }
}

