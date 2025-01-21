using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatusEffectSO : ScriptableObject
{
    private protected EntityStatusEffector entityStatusEffectorOwner; // the EntityStatusEffector that the status effect is applied to
    private protected Entity entity; // the entity that the status effect is applied to
    private protected GameObject source; // the source game object that applied the status effect

    [field: Header("Status Effect: Settings")]
    [field: SerializeField] public bool Stackable { get; protected set; } // if the status effect can stack with itself (all augments should be stackable)

    /// <summary>
    /// Initializes the status effect with the specified owner and source.
    /// </summary>
    /// <param name="owner">The entity status effector owner.</param>
    /// <param name="source">The source game object.</param>
    public void Init(EntityStatusEffector owner, GameObject source)
    {
        entityStatusEffectorOwner = owner;
        entity = owner.GetComponent<Entity>();

        OnApply();
    }

    /// <summary>
    /// Called when the status effect is applied.
    /// Override this function if you want to customize the OnApply behavior.
    /// </summary>
    private protected virtual void OnApply()
    {
        //Debug.Log($"{name} applied on {entityStatusEffectorOwner.gameObject.name}");
    }

    /// <summary>
    /// Updates the status effect.
    /// Override this function if you want to customize the update behavior.
    /// </summary>
    public virtual void Update()
    {

    }

    /// <summary>
    /// Called when the status effect expires.
    /// Removes the status effect from the owner.
    /// Permanent status effects should not expire and are cancelled instead.
    /// Override this function if you want to customize the OnExpire behavior.
    /// </summary>
    private protected virtual void OnExpire()
    {
        entityStatusEffectorOwner.RemoveStatusEffect(GetType(), false);

        //Debug.Log($"{name} Expired");
    }

    /// <summary>
    /// Cancels the status effect.
    /// Override this function if you want to customize the Cancel behavior.
    /// </summary>
    public virtual void Cancel()
    {
        //Debug.Log($"{name} Canceled");
    }

    /// <summary>
    /// Overrides the current status effect with a new status effect of the specified type.
    /// Called once when a status effect is stackable and a new status effect of the same type is applied.
    /// </summary>
    /// <param name="newStatusEffect">The new status effect to override with.</param>
    public void Stack(StatusEffectSO newStatusEffect)
    {
        if (newStatusEffect.GetType() != GetType())
        {
            Debug.LogError($"Cannot override {name} with a different status effect type.");
            return;
        }

        OnStack(newStatusEffect);
    }

    /// <summary>
    /// Called when the status effect is stacked with a new status effect of the same type.
    /// Override if you want to add custom behavior when overriding the status effect.
    /// </summary>
    /// <param name="newStatusEffect">The new status effect to override with.</param>
    private protected virtual void OnStack(StatusEffectSO newStatusEffect)
    {

    }
}
