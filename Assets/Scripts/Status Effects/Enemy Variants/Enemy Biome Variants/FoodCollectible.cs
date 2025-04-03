using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamscape.Abilities;using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class FoodCollectible : CastedAbility, IPoolableObject {
    [Header("Food Collectible Settings")]
    [SerializeField] private List<GameObject> foodPrefabs;

    [SerializeField] private float spawnYOffset = 1f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatHeight = 1f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float delayUntilConsumable = 1f;
    [SerializeField] private HealRegenStatusEffectSO healRegenStatusEffect;

    Vector3 startPosition;
    private bool canConsume = false;
    private Coroutine enableConsumptionCoroutine;
    private Vector3 originalSize;
    
    override protected private void OnSpawn() {
        originalSize = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(originalSize, delayUntilConsumable * .75f).SetEase(Ease.OutElastic);
        startPosition = transform.position;
        // Choose a random food prefab
        foodPrefabs[Random.Range(0, foodPrefabs.Count)].SetActive(true);
        enableConsumptionCoroutine = StartCoroutine(DelayBeforeConsumable(delayUntilConsumable));
    }

    override protected private void OnOnDisable() {
        canConsume = false;
        transform.localScale = originalSize;
        foreach (GameObject obj in foodPrefabs) {
            obj.SetActive(false);
        }
        if (enableConsumptionCoroutine != null) {
            StopCoroutine(enableConsumptionCoroutine);
            enableConsumptionCoroutine = null;
        }
    }
    
    void Update() {
        float newY = startPosition.y + spawnYOffset + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime), Space.Self);
    }

    private void OnTriggerEnter(Collider other) {
        if (!canConsume) return;
        Entity hitEntity = other.GetComponent<Entity>();
        if (hitEntity == null) hitEntity = other.GetComponentInParent<Entity>();
        if (hitEntity == null) return;
        
        if (hitEntity.CurrentState == hitEntity.EntityDeathState) return; // if theyre already dying
        if (hitEntity.Team == casterEntity.Team) return; // casterEntity is Enemy Entity
        
        // Apply food effect
        EntityStatusEffector.TryApplyStatusEffect(hitEntity.gameObject, healRegenStatusEffect, gameObject);
        
        
        // Delete collectible after being consumed
        DestroyAndRelease();
        
    }

    private IEnumerator DelayBeforeConsumable(float delay = 1f) {
        yield return new WaitForSeconds(delay);
        canConsume = true;
    }
   
    

}