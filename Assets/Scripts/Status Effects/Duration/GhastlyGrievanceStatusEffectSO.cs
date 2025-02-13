using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhastlyGrievanceStatusEffectSO : DurationStatusEffectSO
{
    private protected override void OnApply()
    {
        base.OnApply();

        entity.OnEntityTakeDamage += Entity_OnEntityTakeDamage;
    }

    private void Entity_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        throw new NotImplementedException();
    }
}
