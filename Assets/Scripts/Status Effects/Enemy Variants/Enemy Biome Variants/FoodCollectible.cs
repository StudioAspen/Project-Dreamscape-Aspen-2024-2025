using System;
using System.Collections.Generic;
using DG.Tweening;
using Dreamscape.Abilities;using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class FoodCollectible : CastedAbility, IPoolableObject {
    [Header("Food Collectible Settings")]
    [SerializeField] private List<GameObject> foodPrefabs;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatHeight = 1f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private HealRegenStatusEffectSO healRegenStatusEffect;

    Vector3 startPosition;
    
    override protected private void OnSpawn() {
        startPosition = transform.position;
        // Choose a random food prefab
        foodPrefabs[Random.Range(0, foodPrefabs.Count)].SetActive(true);
    }

    override protected private void OnOnDisable() {
        throw new System.NotImplementedException();
    }
    
    void Update() {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime), Space.Self);
    }

    private void OnTriggerEnter(Collider other) {
        Entity hitEntity = other.GetComponent<Entity>();
        if (hitEntity == null) hitEntity = other.GetComponentInParent<Entity>();
        if (hitEntity == null) return;
        
        if (hitEntity.CurrentState == hitEntity.EntityDeathState) return; // if theyre already dying
        if (hitEntity.Team == casterEntity.Team) return; // if theyre on the same team
        
        // Apply food effect
        EntityStatusEffector.TryApplyStatusEffect(hitEntity.gameObject, healRegenStatusEffect, gameObject);
        
    }
    

}