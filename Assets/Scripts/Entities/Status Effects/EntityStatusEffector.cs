using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatusEffector : MonoBehaviour
{
    public StatusEffectSO TestStatusEffect;

    public Dictionary<Type, StatusEffectSO> CurrentStatusEffects { get; private set; } = new Dictionary<Type, StatusEffectSO>();

    private void Update()
    {
        UpdateStatusEffects();

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            OverrideAndExtendStatusEffect(TestStatusEffect, null);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ApplyStatusEffect(TestStatusEffect, null);
        }
    }

    /// <summary>
    /// Updates all the current status effects of the entity.
    /// </summary>
    private void UpdateStatusEffects()
    {
        foreach (StatusEffectSO statusEffect in new List<StatusEffectSO>(CurrentStatusEffects.Values))
        {
            statusEffect.Update();
            Debug.Log($"Update {statusEffect.name}");
        }
    }

    /// <summary>
    /// Adds a copy of the status effect to the entity.
    /// Will always cancel the old status effect if it exists.
    /// </summary>
    /// <param name="newStatusEffect">The new status effect to override or apply.</param>
    /// <param name="source">The source GameObject that applies the status effect.</param>
    public void ApplyStatusEffect(StatusEffectSO newStatusEffect, GameObject source)
    {
        RemoveStatusEffect(newStatusEffect.GetType(), true);

        StatusEffectSO newStatusEffectRuntimeCopy = Instantiate(newStatusEffect);
        newStatusEffectRuntimeCopy.Init(this, source);

        CurrentStatusEffects.Add(newStatusEffect.GetType(), newStatusEffectRuntimeCopy);
    }

    /// <summary>
    /// Overrides and extends the specified status effect on the entity.
    /// If the status effect does not exist, it will be applied.
    /// </summary>
    /// <param name="newStatusEffect">The new status effect to override or apply.</param>
    /// <param name="source">The source GameObject that applies the status effect.</param>
    public void OverrideAndExtendStatusEffect(StatusEffectSO newStatusEffect, GameObject source)
    {
        if (!CurrentStatusEffects.ContainsKey(newStatusEffect.GetType()))
        {
            ApplyStatusEffect(newStatusEffect, source);
            return;
        }

        StatusEffectSO currentStatusEffect = CurrentStatusEffects[newStatusEffect.GetType()];
        currentStatusEffect.Override(newStatusEffect); // extend and override
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
}
