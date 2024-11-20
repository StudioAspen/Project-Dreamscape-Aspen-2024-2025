using DG.Tweening;
using KBCore.Refs;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Burning Rage Stacks")]
public class BurningRageStatusEffectSO : TickStatusEffectSO
{
    [field: Header("Burning Rage Stacks: Settings")]
    [field: SerializeField] public int DamagePerTick { get; private set; } = 1;
    [field: SerializeField] public int MaxStacks { get; private set; } = 5;
    [field: SerializeField] public float MultipleStackMultiplier { get; private set; } = 0;
    private int currentStacks;

    private protected override void OnApply()
    {
        base.OnApply();
        currentStacks = 1;
        entity.TweenTintEntity(new Color(((float)(currentStacks)/MaxStacks), 0,0));
    }

    private protected override void OnTick()
    {
        base.OnTick();

        if(DamagePerTick <= 0) { return; }
        entity.TakeDamageWithoutState(DamagePerTick, entity.GetRandomPositionOnCollider(), source);
    }

    private protected override void OnExpire()
    {
        entity.TweenUnTintEntity();

        base.OnExpire();
    }

    // for when a status is stacking
    public override bool Override(StatusEffectSO newStatusEffect)
    {
        if (!base.Override(newStatusEffect)) return false;
        DamagePerTick = (int)(currentStacks * MultipleStackMultiplier);
        if (currentStacks + 1 >= MaxStacks) { return false; }
        currentStacks++;
        entity.TweenTintEntity(new Color(((float)(currentStacks) / MaxStacks), 0, 0));
        return true;
    }
}
