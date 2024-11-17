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
            AddStatusEffect(TestStatusEffect, null);
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
    /// Will always delete the old status effect if it exists.
    /// </summary>
    /// <typeparam name="T">The type of the status effect.</typeparam>
    /// <param name="newStatusEffect">The new status effect to add.</param>
    public void AddStatusEffect(StatusEffectSO newStatusEffect, GameObject source)
    {
        RemoveStatusEffect(newStatusEffect.GetType(), true);

        StatusEffectSO newStatusEffectRuntimeCopy = Instantiate(newStatusEffect);
        newStatusEffectRuntimeCopy.Init(this, source);

        CurrentStatusEffects.Add(newStatusEffect.GetType(), newStatusEffectRuntimeCopy);
    }

    /// <summary>
    /// Removes the specified status effect from the entity.
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
