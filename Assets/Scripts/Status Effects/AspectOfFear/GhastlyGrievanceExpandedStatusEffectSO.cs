using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhastlyGrievanceExpandedStatusEffectSO : StatusEffectSO
{
    [field: SerializeField] public float defaultGrievanceRadius { get; private set; } = 5f;

    private protected override void OnApply()
    {
        base.OnApply();

        entity.OnEntityDeath += Entity_OnEntityDeath;
    }

    private protected override void OnExpire()
    {
        entity.OnEntityDeath -= Entity_OnEntityDeath;
        base.OnExpire();
    }

    public override void Cancel()
    {
        entity.OnEntityDeath -= Entity_OnEntityDeath;
        base.Cancel();
    }

    private void Entity_OnEntityDeath(GameObject @object)
    {
        Vector3 grievancePosition = entity.GetColliderCenterPosition();

        List<Entity> enemyList = Entity.GetEntitiesThroughAOE(grievancePosition, defaultGrievanceRadius, false);
        
        int grievanceDamage = Mathf.RoundToInt(entity.CurrentHealth / enemyList.Count);

        foreach(Entity enemy in enemyList)
        {
            if (enemy == entity) continue;

            if(enemy.Team != entity.Team) continue;

            enemy.TakeDamage(grievanceDamage, enemy.CharacterController.ClosestPointOnBounds(grievancePosition), source);
        }
    }
}
