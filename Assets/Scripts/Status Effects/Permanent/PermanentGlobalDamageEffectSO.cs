using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Permanent Global Damage Upgrade")]
public class PermanentGlobalDamageEffectSO : StatusEffectSO
{
    [field: Header("Permanent Damage Upgrade Status Effect: Settings")]
    [field: SerializeField] public float GlobalDamageMultiplierIncrease { get; private set; } = 1.1f;

    private protected override void OnApply()
    {
        base.OnApply();

        entity.SetDamageModifier(entity.DamageModifier * GlobalDamageMultiplierIncrease); // apply the new damage multiplier
    }

    public override void Cancel()
    {
        base.Cancel();

        entity.SetDamageModifier(entity.DamageModifier / GlobalDamageMultiplierIncrease); // undo the damage multiplier
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        PermanentGlobalDamageEffectSO overridingStatusEffect = newStatusEffect as PermanentGlobalDamageEffectSO;

        entity.SetDamageModifier(entity.DamageModifier / GlobalDamageMultiplierIncrease); // undo the damage multiplier
        GlobalDamageMultiplierIncrease *= overridingStatusEffect.GlobalDamageMultiplierIncrease; // update the damage multiplier
        entity.SetDamageModifier(entity.DamageModifier * GlobalDamageMultiplierIncrease); // reapply the new updated damage multiplier
    }
}

