using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Burn")]
public class BurnStatusEffectSO : TickStatusEffectSO
{
    EntityRendererManager entityRendererManager;

    [field: Header("Burn: Settings")]
    [field: SerializeField] public int DamagePerTick { get; private set; } = 1;
    [field: SerializeField] public bool HasExtraTickOnApply { get; private set; }

    private protected override void OnApply()
    {
        base.OnApply();

        entityRendererManager = entity.GetComponent<EntityRendererManager>();

        if(entityRendererManager) entityRendererManager.TweenTint(Color.red);

        if(HasExtraTickOnApply) entity.TakeDamage(DamagePerTick, entity.GetRandomPositionOnCollider(), source, false, true);
    }

    private protected override void OnTick()
    {
        base.OnTick();

        entity.TakeDamage(DamagePerTick, entity.GetRandomPositionOnCollider(), source, false);
    }

    private protected override void OnExpire()
    {
        if (entityRendererManager) entityRendererManager.TweenUnTint();

        base.OnExpire();
    }

    public override void Cancel()
    {
        if (entityRendererManager) entityRendererManager.ResetTint();

        base.Cancel();
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        base.OnStack(newStatusEffect);

        BurnStatusEffectSO overridingStatusEffect = newStatusEffect as BurnStatusEffectSO;

        DamagePerTick = overridingStatusEffect.DamagePerTick;
        HasExtraTickOnApply = overridingStatusEffect.HasExtraTickOnApply;
    }
}
