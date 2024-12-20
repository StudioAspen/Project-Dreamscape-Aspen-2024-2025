using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Permanent Speed")]
public class PermanentSpeedStatusEffectSO : StatusEffectSO
{
    [field: Header("Permanent Speed Status Effect: Settings")]
    [field: SerializeField] public float SpeedMultiplier { get; private set; } = 1.1f;

    private protected override void OnApply()
    {
        base.OnApply();

        entity.SetStatusSpeedModifier(entity.StatusSpeedModifier * SpeedMultiplier); // apply the new speed multiplier
    }

    public override void Cancel()
    {
        base.Cancel();

        entity.SetStatusSpeedModifier(entity.StatusSpeedModifier / SpeedMultiplier); // undo the speed multiplier
    }

    public override bool Override(StatusEffectSO newStatusEffect)
    {
        if (!base.Override(newStatusEffect)) return false;

        entity.SetStatusSpeedModifier(entity.StatusSpeedModifier / SpeedMultiplier); // undo the speed multiplier
        SpeedMultiplier *= (newStatusEffect as PermanentSpeedStatusEffectSO).SpeedMultiplier; // update the speed multiplier
        entity.SetStatusSpeedModifier(entity.StatusSpeedModifier * SpeedMultiplier); // reapply the new updated speed multiplier

        return true;
    }
}

