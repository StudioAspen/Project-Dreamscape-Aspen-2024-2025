// using System.Collections;
using DG.Tweening;
using KBCore.Refs;
using System.Collections.Generic;
using UnityEngine;

public class VolcanicEntity : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private Entity entity;
    
    [Header("Explosions Settings")]
    [SerializeField, Self] private float explosionForce = 1000f;
    [SerializeField, Self] private float explosionRadius = 10f;
    Collider[] colliders = new Collider[20];


    public void Update()
    {

        if(entity.CurrentState == entity.EntityDeathState)
        {
            Debug.Log("BOOM");
            explosion();
        }
    }

    private void explosion()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders);
        Debug.Log(transform.position);
        if(numColliders > 0)
        {
            Debug.Log("add colliders to array");
            for (int i = 0; i < numColliders; i++)
            {
                Debug.Log(numColliders);   
                if (colliders[i].TryGetComponent(out Rigidbody rb))
                {
                    
                }
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

}
