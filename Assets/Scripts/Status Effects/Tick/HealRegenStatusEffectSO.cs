using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Health Regeneration")]
public class HealRegenStatusEffectSO : TickStatusEffectSO
{
    EntityRendererManager entityRendererManager;

    [field: Header("Health Regen: Settings")]
    [field: SerializeField] public int HealingPerTick { get; private set; } = 1;
    [field: SerializeField] public bool HasExtraTickOnApply { get; private set; }

    private protected override void OnApply()
    {
        base.OnApply();

        entityRendererManager = entity.GetComponent<EntityRendererManager>();

        if(entityRendererManager) entityRendererManager.TweenTint(Color.green);

        if(HasExtraTickOnApply) entity.Heal(HealingPerTick, true);
    }

    private protected override void OnTick()
    {
        base.OnTick();

        entity.Heal(HealingPerTick, true);
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

        HealRegenStatusEffectSO overridingStatusEffect = newStatusEffect as HealRegenStatusEffectSO;

        HealingPerTick = overridingStatusEffect.HealingPerTick;
        HasExtraTickOnApply = overridingStatusEffect.HasExtraTickOnApply;
    }
}
