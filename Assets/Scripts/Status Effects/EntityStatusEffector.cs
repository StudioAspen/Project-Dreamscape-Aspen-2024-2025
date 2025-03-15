using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityStatusEffector : MonoBehaviour
{
    private Entity entity;

    public Dictionary<Type, StatusEffectSO> CurrentStatusEffects { get; private set; } = new Dictionary<Type, StatusEffectSO>();

    private void Awake()
    {
        entity = GetComponent<Entity>();
    }

    private void OnEnable()
    {
        entity.OnEntityDestroyed += Entity_OnEntityDestroyed;
    }

    private void OnDisable()
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
            if (!CurrentStatusEffects.ContainsKey(newStatusEffect.GetType()))
            {
                return ApplyNotStackedStatusEffect(newStatusEffect, source);
            }

            StatusEffectSO currentStatusEffect = CurrentStatusEffects[newStatusEffect.GetType()];
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
        RemoveStatusEffect(newStatusEffect.GetType(), true);

        StatusEffectSO newStatusEffectRuntimeCopy = Instantiate(newStatusEffect);
        newStatusEffectRuntimeCopy.Init(this, source);

        CurrentStatusEffects.Add(newStatusEffect.GetType(), newStatusEffectRuntimeCopy);

        return newStatusEffectRuntimeCopy;
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
    /// <param name="target">The target gameObject to apply the status effect to.</param>
    /// <param name="statusEffect">The status effect to apply.</param>
    /// <param name="source">The source responsible for applying the effect.</param>
    /// <returns>The status effect applied or null if failed</returns>
    public static StatusEffectSO TryApplyStatusEffect(GameObject target, StatusEffectSO statusEffect, GameObject source)
    {
        EntityStatusEffector statusEffector = target.GetComponent<EntityStatusEffector>();
        if (statusEffector == null) return null;

        return statusEffector.ApplyStatusEffect(statusEffect, source);
    }

    /// <summary>
    /// Tries to get the status effect of the specified type from the target object.
    /// </summary>
    /// <param name="statusEffectType">The type of the status effect.</param>
    /// <returns>The status effect of the specified type, or null if it doesn't exist.</returns>
    public static T TryGetStatusEffect<T>(GameObject target) where T : StatusEffectSO
    {
        EntityStatusEffector statusEffector = target.GetComponent<EntityStatusEffector>();
        if (statusEffector == null) return null;

        return statusEffector.TryGetStatusEffect<T>();
    }

    /// <summary>
    /// Tries to get the status effect of the specified type from the entity.
    /// </summary>
    /// <param name="statusEffectType">The type of the status effect.</param>
    /// <returns>The status effect of the specified type, or null if it doesn't exist.</returns>
    public T TryGetStatusEffect<T>() where T : StatusEffectSO
    {
        if (CurrentStatusEffects.ContainsKey(typeof(T)))
        {
            return CurrentStatusEffects[typeof(T)] as T;
        }

        return null;
    }

    /// <summary>
    /// Checks whether the entity has a status effect of type T
    /// </summary>
    /// <param name="target">The object to check into.</param>
    /// <typeparam name="T">The type of the status effect you're searching for</typeparam>
    /// <returns></returns>
    public static bool HasStatusEffect<T>(GameObject target) where T : StatusEffectSO
    {
        EntityStatusEffector statusEffector = target.GetComponent<EntityStatusEffector>();
        if (statusEffector == null) return false;

        return statusEffector.HasStatusEffect<T>();
    }

    /// <summary>
    /// Checks whether the entity has a status effect of type T
    /// </summary>
    /// <typeparam name="T">The type of the status effect you're searching for</typeparam>
    /// <returns></returns>
    public bool HasStatusEffect<T>() where T : StatusEffectSO
    {

        return CurrentStatusEffects.ContainsKey(typeof(T));
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
