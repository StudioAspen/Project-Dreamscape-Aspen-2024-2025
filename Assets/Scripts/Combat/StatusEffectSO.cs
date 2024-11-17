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

    [Header("Status Effect: Settings")]
    [SerializeField] private protected float duration = 1f;
    private protected float remainingDuration;

    public void Init(EntityStatusEffector owner, GameObject source)
    {
        entityStatusEffectorOwner = owner;
        entity = owner.GetComponent<Entity>();

        remainingDuration = duration;

        OnApply();
    }

    private protected virtual void OnApply()
    {
        Debug.Log($"{name} applied on {entityStatusEffectorOwner.gameObject.name}");
    }

    public virtual void Update()
    {
        remainingDuration -= Time.deltaTime;

        if(remainingDuration <= 0)
        {
            OnExpire();
        }
    }

    private protected virtual void OnExpire()
    {
        entityStatusEffectorOwner.RemoveStatusEffect(GetType(), false);

        Debug.Log($"{name} Expired");
    }

    public virtual void Cancel()
    {
        Debug.Log($"{name} Canceled");
    }
}
