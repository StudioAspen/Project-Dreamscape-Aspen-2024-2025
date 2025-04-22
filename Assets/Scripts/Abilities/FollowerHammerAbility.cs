using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamscape.Abilities;
using UnityEngine;

public class FollowerHammerAbility : CastedAbility
{
    private Rigidbody rigidBody;

    [SerializeField] private Transform hammerTip;
    [SerializeField] private float targetDetectedAOE = 100f;
    [SerializeField] private float initialThrowForce = 10f; 
    [SerializeField] private float moveForce = 3f; // how fast the hammer moves towards the enemy
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float correctionMagnetSpeed = 3f; // how fast the hammer slowly corrects its position to the enemy
    [SerializeField] private float damageMultiplier = 1f; // dmg hammer does
    [SerializeField] private float maxLifeTime = 60f; // dmg hammer does

    private List<Entity> enemyList;
    private Entity currentTarget;
    private HashSet<Entity> hitEnemies;

    private float lifeTimer;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private protected override void OnSpawn()
    {
        currentTarget = null;
        enemyList = new();
        hitEnemies = new();
        lifeTimer = 0;

        rigidBody.AddForceAtPosition(casterEntity.transform.forward * initialThrowForce, hammerTip.position, ForceMode.Impulse); // throw the hammer forward

        // populate list and grab all non-dead entities nearby, and remove player
        List<Entity> nearbyEnemies = Entity.GetEntitiesThroughAOE(casterEntity.transform.position, targetDetectedAOE, false);
        foreach(Entity nearbyEntity in nearbyEnemies)
        {
            if (nearbyEntity == casterEntity) continue; // skip the player
            if (nearbyEntity.Team == casterEntity.Team) continue; // skip allies
            enemyList.Add(nearbyEntity);
        }

        if(enemyList.Count == 0)
        {
            DestroyAndRelease();
            return;
        }
    }

    private protected override void OnOnDisable()
    {

    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if(lifeTimer > maxLifeTime)
        {
            DestroyAndRelease();
            return;
        }

        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed); // clamp the speed of the hammer

        if (enemyList.Count == 0)
        {
            DestroyAndRelease();
            return;
        }

        // Remove any null entities from the list
        foreach(Entity enemy in new List<Entity>(enemyList))
        {
            if (enemy == null || enemy.CurrentState == enemy.EntityDeathState)
            {
                enemyList.Remove(enemy);
            }
        }

        // Check again if the list is empty after removing null entities
        if (enemyList.Count == 0)
        {
            DestroyAndRelease();
            return;
        }

        // Get closest entity to the this hammer
        Entity closestEnemy = enemyList.OrderBy(e => Vector3.Distance(transform.position, e.GetColliderCenterPosition())).FirstOrDefault();
        if (closestEnemy == null)
        {
            DestroyAndRelease();
            return;
        }

        currentTarget = closestEnemy;

        transform.position = Vector3.MoveTowards(transform.position, currentTarget.GetColliderCenterPosition(), correctionMagnetSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (currentTarget == null) return;

        Vector3 moveDirection = (currentTarget.GetColliderCenterPosition() - transform.position).normalized;
        rigidBody.AddForceAtPosition(moveForce * moveDirection, hammerTip.position, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckForEntityHit(other);
    }

    private void CheckForEntityHit(Collider other)
    {
        Entity hitEntity = other.GetComponent<Entity>();
        if (hitEntity == null) hitEntity = other.GetComponentInParent<Entity>();
        if (hitEntity == null) return;

        if (hitEntity.CurrentState == hitEntity.EntityDeathState) return; // if theyre already dying
        if (hitEntity.Team == casterEntity.Team) return; // if theyre on the same team
        if (hitEnemies.Contains(hitEntity)) return; // if we already hit them
        hitEnemies.Add(hitEntity); // add them to the hit list

        casterEntity.DealDamageToOtherEntity(hitEntity, casterEntity.CalculateDamage(damageMultiplier), transform.position);
        if(enemyList.Contains(hitEntity)) enemyList.Remove(hitEntity); // remove the entity from the list
    }
}
