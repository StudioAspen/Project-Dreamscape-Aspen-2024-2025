using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ghastly Grievance", menuName = "Status Effect/Aspect of Fear/Passive A/Ghastly Grievance")]
public class GhastlyGrievanceStatusEffectSO : DurationStatusEffectSO
{
    [field: Header("Ghastly Grievance Stacks: Settings")]
    [field: SerializeField] public float DamageThreshold { get; private set; } = 0.1f; 
    private protected override void OnApply()
    {
        base.OnApply();

        entity.OnEntityTakeDamage += Entity_OnEntityTakeDamage;
    }

    private protected override void OnExpire()
    {
        entity.OnEntityTakeDamage -= Entity_OnEntityTakeDamage;

        base.OnExpire();
    }

    public override void Cancel()
    {
        entity.OnEntityTakeDamage -= Entity_OnEntityTakeDamage;
        base.Cancel();
    }

    private void Entity_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        if (entity.CurrentHealth < Mathf.RoundToInt(entity.MaxHealth.GetFloatValue() * DamageThreshold))
        {
            entity.Kill(source);
        }
    }
}
