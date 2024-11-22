// using System.Collections;
using DG.Tweening;
using KBCore.Refs;
// using System.Collections.Generic;
// using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class VolcanicEntity : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private Entity entity;
    
    [Header("Explosions Settings")]
    [SerializeField, Self] private Vector2Int explosionContactDamageRange  = new Vector2Int(20, 30);
    [SerializeField, Self] public int explosionDamage = 5;
    [SerializeField, Self] public float explosionForce = 10f;
    [SerializeField, Self] private float explosionRadius = 100f;
    [SerializeField, Self] private float stunDuration = 3;
    Collider[] colliders = new Collider[20];


    public void Update()
    {

        if(entity.CurrentState == entity.EntityDeathState)
        {
            Debug.Log("BOOM");
            CheckCollisions();
            
        }
    }

    private void CheckCollisions()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        if(hits == null) return;
        if(hits.Length == 0) return;

        List<Collider> orderedHits = hits.OrderBy(hit => entity.Distance(hit.ClosestPoint(entity.GetColliderCenterPosition()))).ToList();

        foreach(Collider hit in orderedHits)
        {
            // if(IsOwnDamageableEntityCollider(hit)) continue;

            if(DidEntityExplosionDamageFriendlyEntity(hit, out Entity friendlyEntity))
            {

                Debug.Log("friendly, leave alone");
                
            }
            if (DidEntityExplosionHitEnemyEntity(hit, out Entity enemyEntity))
            {
                Debug.Log("ENEMY DEAL DAMAGE");

                Vector3 flingDirection = enemyEntity.GetColliderCenterPosition() - entity.transform.position;
                TryFlingEntity(enemyEntity, flingDirection, explosionForce, stunDuration);
                // entity.GetRandomDamageFromRange(explosionContactDamageRange)
                enemyEntity.TakeDamageWithoutState( explosionDamage, hit.ClosestPoint(entity.GetColliderCenterPosition()), entity.gameObject);
                return;
            }
            
        }
    }

    private void TryFlingEntity(Entity entity, Vector3 direction, float force, float stunDuration)
    {
        if(entity.CurrentState == entity.EntityDeathState) return;
        if(entity.CurrentState == entity.EntityFlingState) return;
        if (entity.GetType() == typeof(Charger)) return;

        entity.EntityFlingState.SetFlingSettings(direction, force, stunDuration);
        entity.ChangeState(entity.EntityFlingState);
    }

    private bool DidEntityExplosionDamageFriendlyEntity(Collider hit, out Entity entity)
    {
        entity = hit.GetComponentInParent<Entity>();

        if(entity == null) return false;
        if (entity.Team != 1) return false;
        print( $"Entity: {entity} Team: {entity.Team}");
        return true;
    }
    // private bool IsOwnDamageableEntityCollider(Collider hit)
    // {
    //     // check if hit is a child of charger's collider
    //     entity selfEntity = hit.GetComponentInParent<VolcanicEntity>();

    //     if(selfEntity == null) return false;
    //     if (selfEntity == entity) return true;

    //     return false;
    // }

    private bool DidEntityExplosionHitEnemyEntity(Collider hit, out Entity entity)
    {
        entity = hit.GetComponentInParent<Entity>();

        if (entity == null) return false;
        if (entity.Team == 1) return false;
        
        return true;
    }

}
