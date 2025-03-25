using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ghastly Grievance", menuName = "Status Effect/Aspect of Fear/Passive A/Ghastly Grievance")]
public class ExtendedDebuffStatusEffectSO : DurationStatusEffectSO
{
    private AspectOfFearPassiveAStatusEffectSO fearPassiveOwner;

    [field: Header("Ghastly Grievance Stacks: Settings")]
    [field: SerializeField] public Marker Marker { get; private set; }
    [field: SerializeField] public float BaseExecuteThreshold { get; private set; } = 0.1f;
    [field: SerializeField] public float ExecuteThresholdPerStack { get; private set; } = 0.05f;
    [field: SerializeField] public int MaxStacks { get; private set; } = 3;
    private Marker markerInstance;
    private int currentStacks;

    [field: Header("Ghastly Grievance Expanded: Settings")]
    [field: SerializeField] public float ExecuteThresholdPerExtraDebuff { get; private set; } = 0f; // TODO
    [field: SerializeField] public float ExecutionExplosionRadius { get; private set; } = 0f;

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        currentStacks = 1;
        markerInstance = Instantiate(Marker, entity.GetEntityTopPosition() + Vector3.up, Quaternion.identity, entity.transform);
        markerInstance.transform.localScale = (0.05f + 0.05f * currentStacks) * Vector3.one;

        entity.OnEntityTakeDamage += Entity_OnEntityTakeDamage;

        // Get the passive from the player who applied this effect and try to add to the counter
        fearPassiveOwner = EntityStatusEffector.TryGetStatusEffect<AspectOfFearPassiveAStatusEffectSO>(source);
        if (fearPassiveOwner != null) fearPassiveOwner.AddSkulledEntity(1);

        // Check for execution threshold on apply
        Entity_OnEntityTakeDamage(0, entity.GetColliderCenterPosition(), source);
    }

    private protected override void OnExpire()
    {
        entity.OnEntityTakeDamage -= Entity_OnEntityTakeDamage;
        if (markerInstance != null) Destroy(markerInstance.gameObject);

        if (fearPassiveOwner != null) fearPassiveOwner.AddSkulledEntity(-1);
        base.OnExpire();
    }

    public override void Cancel()
    {
        entity.OnEntityTakeDamage -= Entity_OnEntityTakeDamage;
        if (markerInstance != null) Destroy(markerInstance.gameObject);

        if (fearPassiveOwner != null) fearPassiveOwner.AddSkulledEntity(-1);
        base.Cancel();
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        // Check for execution threshold again
        Entity_OnEntityTakeDamage(0, entity.GetColliderCenterPosition(), source);

        // dont include base of this overrided method because we dont want the duration to stack
        ExtendedDebuffStatusEffectSO overridingStatusEffect = newStatusEffect as ExtendedDebuffStatusEffectSO;

        RemainingDuration = overridingStatusEffect.Duration; // Reset duration

        if (currentStacks >= MaxStacks) return; // Don't increase current stacks if already at maxstacks
        currentStacks++;
        if(markerInstance != null) markerInstance.transform.localScale = (0.05f + 0.05f * currentStacks) * Vector3.one;
    }

    private void Entity_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        if (entity.CurrentHealth < Mathf.RoundToInt(entity.MaxHealth.GetFloatValue() * GetFinalExecuteThreshold()))
        {
            //Debug.Log($"{entity.gameObject.name} reached {GetFinalExecuteThreshold()} threshold of health, executing");
            TryExecutionAOEExplosion(entity.CurrentHealth, entity.GetColliderCenterPosition());
            entity.Kill(source);
        }
    }

    /// <summary>
    /// Calculates the final execute threshold based on current stacks
    /// </summary>
    /// <returns>The final execution threshold</returns>
    private float GetFinalExecuteThreshold()
    {
        return BaseExecuteThreshold + currentStacks * ExecuteThresholdPerStack;
    }

    /// <summary>
    /// Attempts to deal the execution damage to nearby allies if this status effect is the exanded version
    /// </summary>
    /// <param name="damage">The damage the AOE will deal</param>
    /// <param name="center">The center of the AOE</param>
    private void TryExecutionAOEExplosion(int damage, Vector3 center)
    {
        if(ExecutionExplosionRadius <= 0) return; // only expanded version has a radius > 0

        List<Entity> nearbyEntities = Entity.GetEntitiesThroughAOE(center, ExecutionExplosionRadius, false);
        foreach(Entity nearbyEntity in nearbyEntities)
        {
            if(entity == nearbyEntity) continue;
            if(entity.Team != nearbyEntity.Team) continue; // if not an ally
            Debug.Log($"Nearby entity {nearbyEntity.gameObject.name}");
            nearbyEntity.TakeDamage(damage, nearbyEntity.CharacterController.ClosestPoint(center), source, false);
        }
        CustomDebug.InstantiateTemporarySphere(center, ExecutionExplosionRadius, 0.5f, new Color(0, 0, 0, 0.5f));
    }
}
