using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Burn")]
public class BurnStatusEffectSO : TickStatusEffectSO
{
    [field: Header("Burn: Settings")]
    [field: SerializeField] public int DamagePerTick { get; private set; } = 1;
    [field: SerializeField] public bool HasExtraTickOnApply { get; private set; }

    private protected override void OnApply()
    {
        base.OnApply();

        entity.TweenTintEntity(Color.red);

        if(HasExtraTickOnApply) entity.TakeDamage(DamagePerTick, entity.GetRandomPositionOnCollider(), source, false);
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

    public override bool OnStack(StatusEffectSO newStatusEffect)
    {
        if (!base.OnStack(newStatusEffect)) return false;

        DamagePerTick = (newStatusEffect as BurnStatusEffectSO).DamagePerTick;
        HasExtraTickOnApply = (newStatusEffect as BurnStatusEffectSO).HasExtraTickOnApply;

        return true;
    }
}
