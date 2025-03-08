using System.Collections;
using System.Collections.Generic;
using Dreamscape.Abilities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class FollowerHammerAbility : CastedAbility
{
    [field: SerializeField] public float TargetDetectedAOE { get; private set; } = 2f;

    private List<Entity> enemyList;
    private float timer;
    private protected override void OnSpawn()
    {
        // populate list and grab all non-dead entities nearby
        enemyList = Entity.GetEntitiesThroughAOE(casterEntity.transform.position, TargetDetectedAOE, false);
        for (int i = 0; i < enemyList.Count; i++) // loop through all entities
        {
            Entity enemy = enemyList[i]; // current entity in the loop

            this.transform.position = enemy.transform.position;
        }
    }

    private protected override void OnOnDisable()
    {
        this.DestroyAndRelease();
    }
}
