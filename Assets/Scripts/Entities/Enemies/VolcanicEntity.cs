using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class VolcanicEntity : MonoBehaviour
{
    private Entity entity;
    
    [Header("Explosion Settings")]
    [SerializeField] public float explosionRadius = 5f;
    [SerializeField] public float explosionPercentDamage = 150f;
    [SerializeField] public float explosionLaunchForce = 15f;
    [SerializeField] public float explosionStunDuration = 2f;

    private void Awake()
    {
        entity = GetComponent<Entity>();
    }

    private void OnEnable()
    {
        entity.OnEntityDeath += Entity_OnEntityDeath;
    }

    private void OnDisable()
    {
        entity.OnEntityDeath -= Entity_OnEntityDeath;
    }

    private void Entity_OnEntityDeath(GameObject killer)
    {
        DamageWithAOE(explosionRadius);
    }

    private void DamageWithAOE(float radius)
    {
        List<Entity> entitiesHit = Entity.GetEntitiesThroughAOE(entity.transform.position, radius, false);

        foreach (Entity entityHit in entitiesHit)
        {
            if (entityHit.Team == entity.Team) continue; // skip friendly entities

            entityHit.TakeDamage(
                entity.CalculateDamage(explosionPercentDamage),
                entityHit.GetComponent<Collider>().ClosestPointOnBounds(entity.transform.position),
                entity.gameObject,
                false);

            Vector3 launchDirection = (entityHit.GetColliderCenterPosition() - entity.transform.position).normalized;

            entityHit.TryChangeToLaunchState(launchDirection, explosionLaunchForce, explosionStunDuration);
        }

        //insert explosion vfx here:
        CustomDebug.InstantiateTemporarySphere(entity.transform.position, radius, 0.25f, new Color(1f, 0, 0, 0.2f));
    }
}
