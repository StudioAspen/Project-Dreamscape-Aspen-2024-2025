using System.Collections;
using System.Collections.Generic;
using Dreamscape.Abilities;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class FollowerHammerAbility : CastedAbility
{
    [SerializeField] public float TargetDetectedAOE { get; private set; } = 50f;
    [SerializeField] public float MoveSpeed { get; private set; } = 3f; // how fast the hammer moves towards the enemy
    [SerializeField] public float MoveDuration { get; private set; } = 2f; // time to reach the target
    [SerializeField] private float damageMultiplier = 1f; // dmg hammer does

    private List<Entity> enemyList;
    private int currentEnemyIndex = 0; // to track which enemy we are targeting

    private protected override void OnSpawn()
    {
        // populate list and grab all non-dead entities nearby, and remove player
        enemyList = Entity.GetEntitiesThroughAOE(casterEntity.transform.position, TargetDetectedAOE, false);
        for(int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] == casterEntity)
                enemyList.RemoveAt(i);
        }

        CustomDebug.InstantiateTemporarySphere(casterEntity.transform.position, TargetDetectedAOE, 0.25f, new Color(1f, 0, 0, 0.2f));
        if (enemyList.Count > 0)
        {
            // Start moving the hammer toward the first enemy
            StartCoroutine(MoveHammerToEnemy());
        }
        else
        {
            OnOnDisable();
        }
    }

    private IEnumerator MoveHammerToEnemy()
    {
        while (currentEnemyIndex < enemyList.Count)
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

    private void Explode(Collider other)
    {
        Entity enemy = other.GetComponent<Entity>();
        if (enemy.Team == casterEntity.Team) return;
        casterEntity.DealDamageToOtherEntity(enemy, casterEntity.CalculateDamage(damageMultiplier), transform.position);
    }

    private void CheckForEntityHit(Collider other)
    {
        Entity hitEntity = other.GetComponent<Entity>();
        if (hitEntity == null) hitEntity = other.GetComponentInParent<Entity>();
        if (hitEntity == null) return;

        if (hitEntity.CurrentState == hitEntity.EntityDeathState) return; // if theyre already dying
        if (hitEntity.Team == casterEntity.Team) return; // if theyre on the same team

        Explode(other);
    }

    private void CheckForWallHit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Ground")) return;
        Explode(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckForEntityHit(other);
        CheckForWallHit(other);
    }

    private protected override void OnOnDisable()
    {
        this.DestroyAndRelease();
    }
}
