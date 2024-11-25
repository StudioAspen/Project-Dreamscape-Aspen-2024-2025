using DG.Tweening;
using KBCore.Refs;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Burn")]
public class BurnStatusEffectSO : TickStatusEffectSO
{
    [field: Header("Burn: Settings")]
    [field: SerializeField] public int DamagePerTick { get; private set; } = 1;

    private protected override void OnApply()
    {
        base.OnApply();

        entity.TweenTintEntity(Color.red);
    }

    private protected override void OnTick()
    {
        base.OnTick();

        entity.TakeDamage(DamagePerTick, entity.GetRandomPositionOnCollider(), source, false);
    }

    private protected override void OnExpire()
    {
        entity.TweenUnTintEntity();

        base.OnExpire();
    }

    public override void Cancel()
    {
        entity.ResetTint();

        base.Cancel();
    }

    public override bool Override(StatusEffectSO newStatusEffect)
    {
        if (!base.Override(newStatusEffect)) return false;

        DamagePerTick = (newStatusEffect as BurnStatusEffectSO).DamagePerTick;

        return true;
    }
}
