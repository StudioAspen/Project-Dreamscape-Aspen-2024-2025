using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Temporary Speed")]
public class TemporarySpeedStatusEffectSO : DurationStatusEffectSO
{
    [field: Header("Permanent Speed Status Effect: Settings")]
    [field: SerializeField] public float SpeedMultiplier { get; private set; } = 1.1f;

    private protected override void OnApply()
    {
        base.OnApply();

        entity.StatusSpeedModifier.AddMultiplier(SpeedMultiplier, this);
    }

    private protected override void OnExpire()
    {
        base.OnExpire();

        entity.StatusSpeedModifier.ClearBuffsFromSource(this);
    }

    public override void Cancel()
    {
        base.Cancel();

        entity.StatusSpeedModifier.ClearBuffsFromSource(this);
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        base.OnStack(newStatusEffect);

        TemporarySpeedStatusEffectSO overridingStatusEffect = newStatusEffect as TemporarySpeedStatusEffectSO;

        entity.StatusSpeedModifier.ClearBuffsFromSource(this);
        SpeedMultiplier *= overridingStatusEffect.SpeedMultiplier;
        entity.StatusSpeedModifier.AddMultiplier(SpeedMultiplier, this);
    }
}