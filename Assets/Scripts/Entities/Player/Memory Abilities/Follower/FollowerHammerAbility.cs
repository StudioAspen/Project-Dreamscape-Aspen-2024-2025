using System.Collections;
using System.Collections.Generic;
using Dreamscape.Abilities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class FollowerHammerAbility : CastedAbility
{
    [field: SerializeField] public float TargetDetectedAOE { get; private set; } = 50f;
    [field: SerializeField] public float MoveSpeed { get; private set; } = 3f; // how fast the hammer moves towards the enemy
    [field: SerializeField] public float MoveDuration { get; private set; } = 2f; // time to reach the target

    private List<Entity> enemyList;
    private int currentEnemyIndex = 0; // to track which enemy we are targeting

    private protected override void OnSpawn()
    {
        Debug.Log("follower ability, spawn hammer");

        // populate list and grab all non-dead entities nearby
        enemyList = Entity.GetEntitiesThroughAOE(casterEntity.transform.position, TargetDetectedAOE, false);

        CustomDebug.InstantiateTemporarySphere(casterEntity.transform.position, TargetDetectedAOE, 0.25f, new Color(1f, 0, 0, 0.2f));
        if (enemyList.Count > 1)
        {
            // Start moving the hammer toward the first enemy
            StartCoroutine(MoveHammerToEnemy());
        }
    }

    private IEnumerator MoveHammerToEnemy()
    {
        Debug.Log("total enemies: " + enemyList.Count);

        while (currentEnemyIndex + 1 < enemyList.Count)
        {
            Entity currentEnemy = enemyList[currentEnemyIndex];
            Vector3 startPos = transform.position;
            Vector3 targetPos = currentEnemy.transform.position;

            float elapsedTime = 0f;

            // move hammer to the current enemy over MoveDuration time
            while (elapsedTime < MoveDuration)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / MoveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the hammer reaches the target position
            transform.position = targetPos;
            currentEnemyIndex++;

            // You can add some delay between each movement (optional)
            yield return new WaitForSeconds(0.5f); // Pause before moving to next enemy
        }

        // If all enemies are targeted, destroy the hammer or finish the ability
        OnOnDisable();
    }

    private protected override void OnOnDisable()
    {
        this.DestroyAndRelease();
    }
}
