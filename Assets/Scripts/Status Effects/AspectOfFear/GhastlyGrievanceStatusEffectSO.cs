using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhastlyGrievanceStatusEffectSO : DurationStatusEffectSO
{
    [field: Header("Ghastly Grievance Stacks: Settings")]
    [field: SerializeField] public float dmgThreshold { get; private set; } = 0.1f; 
    private protected override void OnApply()
    {
        base.OnApply();

        entity.OnEntityTakeDamage += Entity_OnEntityTakeDamage;
    }

    private protected override void OnExpire()
    {
        base.OnExpire();
        entity.OnEntityTakeDamage -= Entity_OnEntityTakeDamage;
    }

    private void Entity_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        if (entity.CurrentHealth - damage < Mathf.RoundToInt(entity.MaxHealth * dmgThreshold))
        {
            entity.Kill(source);
        }
    }
}
