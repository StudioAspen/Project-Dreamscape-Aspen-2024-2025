using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Charge Attack Activated")]
public class ChargeAttackActivatedStatusEffectSO : StatusEffectSO
{
    [field: Header("Charge Attack Activated: Settings")]
    [field: SerializeField] public float MaxChargeDuration { get; private set; } = 3f;

    /// <summary>
    /// Sets the maximum charge duration for the charge attack activated status effect.
    /// </summary>
    /// <param name="newDuration">The new duration value to set.</param>
    public void SetMaxChargeDuration(float newDuration)
    {
        MaxChargeDuration = newDuration;
    }

    private protected override void OnApply()
    {
        base.OnApply();
    }

    public override void Cancel()
    {
        base.Cancel();
    }

    public override bool OnStack(StatusEffectSO newStatusEffect)
    {
        if (!base.OnStack(newStatusEffect)) return false;

        SetMaxChargeDuration((newStatusEffect as ChargeAttackActivatedStatusEffectSO).MaxChargeDuration);

        return true;
    }
}