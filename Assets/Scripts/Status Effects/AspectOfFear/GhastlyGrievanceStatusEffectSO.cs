using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ghastly Grievance", menuName = "Status Effect/Aspect of Fear/Passive A/Ghastly Grievance")]
public class GhastlyGrievanceStatusEffectSO : DurationStatusEffectSO
{
    [field: Header("Ghastly Grievance Stacks: Settings")]
    [field: SerializeField] public float BaseExecuteThreshold { get; private set; } = 0.1f;
    [field: SerializeField] public float ExecuteThresholdPerStack { get; private set; } = 0.05f;
    [field: SerializeField] public int MaxStacks { get; private set; } = 3;
    private int currentStacks;

    [field: Header("Ghastly Grievance Expanded: Settings")]
    [field: SerializeField] public float DamageUpPercentPerStack { get; private set; } = 0f;
    [field: SerializeField] public float ExecuteThresholdPerExtraDebuff { get; private set; } = 0f;
    [field: SerializeField] public float ExecutionExplosionRadius { get; private set; } = 0f;

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        currentStacks = 1;

        entity.OnEntityTakeDamage += Entity_OnEntityTakeDamage;
    }

    private protected override void OnExpire()
    {
        entity.OnEntityTakeDamage -= Entity_OnEntityTakeDamage;

        base.OnExpire();
    }

    public override void Cancel()
    {
        entity.OnEntityTakeDamage -= Entity_OnEntityTakeDamage;

        base.Cancel();
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        // dont include base of this overrided method because we dont want the duration to stack
        GhastlyGrievanceStatusEffectSO overridingStatusEffect = newStatusEffect as GhastlyGrievanceStatusEffectSO;

        RemainingDuration = overridingStatusEffect.Duration; // Reset duration

        if (currentStacks >= MaxStacks) return; // Don't increase current stacks if already at maxstacks
        currentStacks++;
    }

    private void Entity_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        if (entity.CurrentHealth < Mathf.RoundToInt(entity.MaxHealth.GetFloatValue() * GetFinalExecuteThreshold()))
        {
            entity.Kill(source);
        }
    }

    /// <summary>
    /// Calculates the final execute threshold based on current stacks
    /// </summary>
    /// <returns>The final execution threshold</returns>
    private float GetFinalExecuteThreshold()
    {
        return BaseExecuteThreshold + currentStacks * AdditionalExecuteThresholdPerStack;
    }
}
