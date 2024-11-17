using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatusEffectSO : ScriptableObject
{
    private EntityStatusEffector entityStatusEffectorOwner;
    private protected Entity entity;
    private protected GameObject source;

    [field: Header("Status Effect: Settings")]
    [field: SerializeField] public float Duration { get; protected set; } = 1f;
    public float RemainingDuration { get; protected set; }

    /// <summary>
    /// Initializes the status effect with the specified owner and source.
    /// </summary>
    /// <param name="owner">The entity status effector owner.</param>
    /// <param name="source">The source game object.</param>
    public void Init(EntityStatusEffector owner, GameObject source)
    {
        entityStatusEffectorOwner = owner;
        entity = owner.GetComponent<Entity>();

        RemainingDuration = Duration;

        OnApply();
    }

    /// <summary>
    /// Called when the status effect is applied.
    /// Override this function if you want to customize the OnApply behavior.
    /// </summary>
    private protected virtual void OnApply()
    {
        Debug.Log($"{name} applied on {entityStatusEffectorOwner.gameObject.name}");
    }

    /// <summary>
    /// Updates the status effect by decreasing the remaining duration and checking if it has expired.
    /// Override this function if you want to customize the update behavior.
    /// </summary>
    public virtual void Update()
    {
        RemainingDuration -= Time.deltaTime;

        if (RemainingDuration <= 0)
        {
            OnExpire();
        }
    }

    /// <summary>
    /// Called when the status effect expires.
    /// Removes the status effect from the owner.
    /// Override this function if you want to customize the OnExpire behavior.
    /// </summary>
    private protected virtual void OnExpire()
    {
        entityStatusEffectorOwner.RemoveStatusEffect(GetType(), false);

        Debug.Log($"{name} Expired");
    }

    /// <summary>
    /// Cancels the status effect.
    /// Override this function if you want to customize the Cancel behavior.
    /// </summary>
    public virtual void Cancel()
    {
        Debug.Log($"{name} Canceled");
    }

    /// <summary>
    /// Overrides the current status effect with a new status effect by extending the current duration.
    /// Override this function if you want to customize the override behavior.
    /// </summary>
    /// <param name="newStatusEffect">The new status effect to override with.</param>
    public virtual void Override(StatusEffectSO newStatusEffect)
    {
        RemainingDuration += newStatusEffect.Duration;
    }
}
