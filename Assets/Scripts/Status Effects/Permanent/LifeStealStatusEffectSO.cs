using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Life Steal")]
public class LifeStealStatusEffectSO : StatusEffectSO
{
    [field: Header("Life Steal: Settings")]
    [field: SerializeField] public float HealPercent { get; private set; } = 0.1f;

    private protected override void OnApply()
    {
        base.OnApply();

        entity.OnEntityDealDamage += Entity_OnEntityDealDamage;
    }

    public override void Cancel()
    {
        base.Cancel();

        entity.OnEntityDealDamage -= Entity_OnEntityDealDamage;
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        LifeStealStatusEffectSO overridingStatusEffect = newStatusEffect as LifeStealStatusEffectSO;

        HealPercent = overridingStatusEffect.HealPercent;
    }

    private void Entity_OnEntityDealDamage(Entity source, Entity victim, Vector3 hitPoint, int damageValue)
    {
        int healValue = Mathf.RoundToInt(damageValue * HealPercent);

        if(healValue > 0) entity.Heal(healValue);
    }
}
