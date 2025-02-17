using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityStatusEffector : MonoBehaviour
{
    private Entity entity;

    /// <summary>
    /// Stores the entities status effects
    /// Key: Original Copy, Value: Runtime copy
    /// </summary>
    public Dictionary<StatusEffectSO, StatusEffectSO> CurrentStatusEffects { get; private set; } = new Dictionary<StatusEffectSO, StatusEffectSO>();

    private void Awake()
    {
        entity = GetComponent<Entity>();

        entity.OnEntityDestroyed += Entity_OnEntityDestroyed;
    }

    private void OnDestroy()
    {
        entity.OnEntityDestroyed -= Entity_OnEntityDestroyed;
    }

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
    public StatusEffectSO ApplyStatusEffect(StatusEffectSO newStatusEffect, GameObject source)
    {
        if (newStatusEffect.Stackable)
        {
            if (!CurrentStatusEffects.ContainsKey(newStatusEffect))
            {
                return ApplyNotStackedStatusEffect(newStatusEffect, source);
            }

            StatusEffectSO currentStatusEffect = CurrentStatusEffects[newStatusEffect];
            currentStatusEffect.Stack(newStatusEffect); // extend and override

            return currentStatusEffect;
        }
        else
        {
            return ApplyNotStackedStatusEffect(newStatusEffect, source);
        }
    }

    /// <summary>
    /// Applies a non-stackable status effect to the entity.
    /// Removes any existing status effect of the same type and creates a new instance of the status effect.
    /// </summary>
    /// <param name="newStatusEffect">The new status effect to apply.</param>
    /// <param name="source">The source GameObject that applies the status effect.</param>
    private StatusEffectSO ApplyNotStackedStatusEffect(StatusEffectSO newStatusEffect, GameObject source)
    {
        RemoveStatusEffect(newStatusEffect, true);

        StatusEffectSO newStatusEffectRuntimeCopy = Instantiate(newStatusEffect);
        newStatusEffectRuntimeCopy.Init(newStatusEffect, this, source);

        CurrentStatusEffects.Add(newStatusEffect, newStatusEffectRuntimeCopy);

        return newStatusEffectRuntimeCopy;
    }

    /// <summary>
    /// Removes the specified status effect from the entity.
    /// Cancelling the status effect will not trigger OnExpire.
    /// </summary>
    /// <param name="originalStatusEffect">The status effect to remove.</param>
    /// <param name="cancel">Indicates whether to cancel the status effect.</param>
    public void RemoveStatusEffect(StatusEffectSO originalStatusEffect, bool cancel)
    {
        if (CurrentStatusEffects.ContainsKey(originalStatusEffect))
        {
            if (cancel) CurrentStatusEffects[originalStatusEffect].Cancel();

            Destroy(CurrentStatusEffects[originalStatusEffect]);

            CurrentStatusEffects.Remove(originalStatusEffect);
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
    /// Tries to get the status effect of the specified type from the entity using the original copy.
    /// </summary>
    /// <param name="target">The gameObject to get the status effect from.</param>
    /// <param name="originalCopy">The original copy of the status effect.</param>
    /// <typeparam name="T">The type of the status effect you want to get.</typeparam>
    /// <returns>The status effect of the specified type, or null if it doesn't exist.</returns>
    public static T TryGetStatusEffect<T>(GameObject target, StatusEffectSO originalCopy) where T : StatusEffectSO
    {
        EntityStatusEffector statusEffector = target.GetComponent<EntityStatusEffector>();
        if (statusEffector == null) return null;

        return statusEffector.TryGetStatusEffect<T>(originalCopy);
    }

    /// <summary>
    /// Tries to get the status effect of the specified type from the entity using the original copy.
    /// </summary>
    /// <param name="originalCopy">The original copy of the status effect.</param>
    /// <typeparam name="T">The type of the status effect you want to get.</typeparam>
    /// <returns>The status effect of the specified type, or null if it doesn't exist.</returns>
    public T TryGetStatusEffect<T>(StatusEffectSO originalCopy) where T : StatusEffectSO
    {
        if(originalCopy.GetType() != typeof(T))
        {
            Debug.LogError("TryGetStatusEffect: Original copy and type don't match");
            return null;
        }

        if (CurrentStatusEffects.ContainsKey(originalCopy))
        {
            return CurrentStatusEffects[originalCopy] as T;
        }

        return null;
    }

    /// <summary>
    /// Detects whether the status effect of the specified type is applied to the entity.
    /// </summary>
    /// <param name="target">The target gameObject to search for.</param>
    /// <typeparam name="T">The type of the status effect to search for</typeparam>
    /// <returns>Whether the type exists.</returns>
    public static bool HasStatusEffectOfType<T>(GameObject target) where T : StatusEffectSO
    {
        EntityStatusEffector statusEffector = target.GetComponent<EntityStatusEffector>();
        if (statusEffector == null) return false;

        return statusEffector.HasStatusEffectOfType<T>();
    }

    /// <summary>
    /// Detects whether the status effect of the specified type is applied to the entity.
    /// </summary>
    /// <typeparam name="T">The type of the status effect to search for</typeparam>
    /// <returns>Whether the type exists.</returns>
    public bool HasStatusEffectOfType<T>() where T : StatusEffectSO
    {
        foreach(StatusEffectSO originalCopy in CurrentStatusEffects.Keys)
        {
            if(originalCopy.GetType() == typeof(T))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Cancels and removes all current status effects from the entity when it destroyed.
    /// </summary>
    private void Entity_OnEntityDestroyed(Entity entity, GameObject @object)
    {
        for (int i = 0; i < CurrentStatusEffects.Count; i++)
        {
            StatusEffectSO statusEffect = CurrentStatusEffects.Values.ElementAt(i);
            statusEffect.Cancel();
            Destroy(statusEffect);
        }

        CurrentStatusEffects.Clear();
    }
}
