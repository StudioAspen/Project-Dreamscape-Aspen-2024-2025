using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Rage/Passive B")]
public class AspectOfRagePassiveBStatusEffectSO : StatusEffectSO
{
    private PlayerCombat playerCombat;

    [field: Header("Aspect of Rage Passive B: Settings")]
    [field: SerializeField] public ChargeAttackActivatedStatusEffectSO ChargedAttackActivatedStatusEffect { get; private set; }
    [field: SerializeField] public float MaxChargeDuration { get; private set; } = 5f;
    [field: SerializeField] public List<float> ChargeDurationBonusPercentDamages { get; private set; } = new List<float>();

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        ChargeAttackActivatedStatusEffectSO chargeAttackActivatedStatusEffectInstance =
            entityStatusEffectorOwner.ApplyStatusEffect(ChargedAttackActivatedStatusEffect, entity.gameObject) as ChargeAttackActivatedStatusEffectSO;
        chargeAttackActivatedStatusEffectInstance.SetMaxChargeDuration(MaxChargeDuration);

        playerCombat = entity.GetComponent<PlayerCombat>();
        if (playerCombat != null) playerCombat.OnChargeRelease += PlayerCombat_OnChargeRelease;
    }

    public override void Cancel()
    {
        base.Cancel();

        entityStatusEffectorOwner.RemoveStatusEffect(ChargedAttackActivatedStatusEffect.GetType(), true);

        if (playerCombat != null) playerCombat.OnChargeRelease -= PlayerCombat_OnChargeRelease;
    }

    public override bool OnStack(StatusEffectSO newStatusEffect)
    {
        if (!base.OnStack(newStatusEffect)) return false;

        // Set new max charge duration
        EntityStatusEffector.TryGetStatusEffect<ChargeAttackActivatedStatusEffectSO>(entity.gameObject)
            .SetMaxChargeDuration((newStatusEffect as ChargeAttackActivatedStatusEffectSO).MaxChargeDuration);

        // Set new damage bonus values
        ChargeDurationBonusPercentDamages = (newStatusEffect as AspectOfRagePassiveBStatusEffectSO).ChargeDurationBonusPercentDamages;

        return true;
    }

    private void PlayerCombat_OnChargeRelease(int attackInputNumber, float chargeDuration)
    {
        if(ChargeDurationBonusPercentDamages.Count == 0) return;

        float intervalSize = MaxChargeDuration / ChargeDurationBonusPercentDamages.Count;

        for(int i = 0; i < ChargeDurationBonusPercentDamages.Count; i++)
        {
            if(chargeDuration <= intervalSize)
            {
                return;
            }
            if(chargeDuration <= intervalSize * (i + 2))
            {
                playerCombat.GetComponent<Player>().PlayerAttackState.SetExtraPercentDamage(ChargeDurationBonusPercentDamages[i]);
                //Debug.Log($"Charge Duration: {chargeDuration}, {chargeDuration/ChargeDurationBonusPercentDamages.Count}, Bonus: {ChargeDurationBonusPercentDamages[i]}");
                return;
            }
        }
    }
}