using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Temporary Speed")]
public class TemporarySpeedStatusEffectSO : DurationStatusEffectSO
{
    [field: Header("Permanent Speed Status Effect: Settings")]
    [field: SerializeField] public float SpeedMultiplier { get; private set; } = 1.1f;

    private protected override void OnApply()
    {
        base.OnApply();

        entity.SetStatusSpeedModifier(entity.StatusSpeedModifier * SpeedMultiplier);
    }

    private protected override void OnExpire()
    {
        base.OnExpire();

        entity.SetStatusSpeedModifier(entity.StatusSpeedModifier / SpeedMultiplier);
    }

    public override void Cancel()
    {
        base.Cancel();

        entity.SetStatusSpeedModifier(entity.StatusSpeedModifier / SpeedMultiplier);
    }

    public override bool OnStack(StatusEffectSO newStatusEffect)
    {
        if (!base.OnStack(newStatusEffect)) return false;

        entity.SetStatusSpeedModifier(entity.StatusSpeedModifier / SpeedMultiplier);
        SpeedMultiplier *= (newStatusEffect as TemporarySpeedStatusEffectSO).SpeedMultiplier;
        entity.SetStatusSpeedModifier(entity.StatusSpeedModifier * SpeedMultiplier);

        return true;
    }
}