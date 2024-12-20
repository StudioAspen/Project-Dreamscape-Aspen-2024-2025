using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatusEffector : MonoBehaviour
{
    public Dictionary<Type, StatusEffectSO> CurrentStatusEffects { get; private set; } = new Dictionary<Type, StatusEffectSO>();

    private void Update()
    {
        UpdateStatusEffects();
    }

    /// <summary>
    /// Updates all the current status effects of the entity.
    /// </summary>
    private void UpdateStatusEffects()
    {
        foreach (StatusEffectSO statusEffect in new List<StatusEffectSO>(CurrentStatusEffects.Values))
        {
            statusEffect.Update();
        }
    }

    /// <summary>
    /// Adds a copy of the status effect to the entity.
    /// If stackable, overrides and extends the specified status effect on the entity and if the status effect does not exist, it will be applied.
    /// If unstackable, will always cancel the old status effect if it exists.
    /// </summary>
    /// <param name="newStatusEffect">The new status effect to override or apply.</param>
    /// <param name="source">The source GameObject that applies the status effect.</param>
    public void ApplyStatusEffect(StatusEffectSO newStatusEffect, GameObject source)
    {
        if (newStatusEffect.Stackable)
        {
            if (!CurrentStatusEffects.ContainsKey(newStatusEffect.GetType()))
            {
                ApplyNotStackedStatusEffect(newStatusEffect, source);
                return;
            }

            StatusEffectSO currentStatusEffect = CurrentStatusEffects[newStatusEffect.GetType()];
            currentStatusEffect.Override(newStatusEffect); // extend and override
        }
        else
        {
            ApplyNotStackedStatusEffect(newStatusEffect, source);
        }
    }

    /// <summary>
    /// Applies a non-stackable status effect to the entity.
    /// Removes any existing status effect of the same type and creates a new instance of the status effect.
    /// </summary>
    /// <param name="newStatusEffect">The new status effect to apply.</param>
    /// <param name="source">The source GameObject that applies the status effect.</param>
    private void ApplyNotStackedStatusEffect(StatusEffectSO newStatusEffect, GameObject source)
    {
        RemoveStatusEffect(newStatusEffect.GetType(), true);

        StatusEffectSO newStatusEffectRuntimeCopy = Instantiate(newStatusEffect);
        newStatusEffectRuntimeCopy.Init(this, source);

        CurrentStatusEffects.Add(newStatusEffect.GetType(), newStatusEffectRuntimeCopy);
    }

    /// <summary>
    /// Removes the specified status effect from the entity.
    /// Cancelling the status effect will not trigger OnExpire.
    /// </summary>
    /// <param name="statusEffectType">The status effect type to remove.</param>
    /// <param name="cancel">Indicates whether to cancel the status effect.</param>
    public void RemoveStatusEffect(Type statusEffectType, bool cancel)
    {
        if (CurrentStatusEffects.ContainsKey(statusEffectType))
        {
            if (cancel) CurrentStatusEffects[statusEffectType].Cancel();

            Destroy(CurrentStatusEffects[statusEffectType]);

            CurrentStatusEffects.Remove(statusEffectType);
        }
    }

    /// <summary>
    /// Tries to apply a status effect to the target GameObject.
    /// If the target has an EntityStatusEffector component, the status effect will be applied.
    /// </summary>
    public static void TryApplyStatusEffect(GameObject target, StatusEffectSO statusEffect, GameObject source)
    {
        EntityStatusEffector statusEffector = target.GetComponent<EntityStatusEffector>();
        if (statusEffector == null) return;

        statusEffector.ApplyStatusEffect(statusEffect, source);
    }

    /// <summary>
    /// Cancels and removes all current status effects from the entity when it is disabled.
    /// </summary>
    private void OnDisable()
    {
        foreach (StatusEffectSO statusEffect in CurrentStatusEffects.Values)
        {
            statusEffect.Cancel();
            Destroy(statusEffect);
        }

        CurrentStatusEffects.Clear();
    }
}
