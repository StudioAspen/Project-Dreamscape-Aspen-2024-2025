using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityOutOfCombatRegen : MonoBehaviour
{
    private Entity entity;

    [field: Header("Config")]
    [SerializeField] private int healthRegenAmount = 1;
    [SerializeField] private float healthRegenRate = 0.25f;
    [SerializeField] private float durationOutOfCombatToRegen = 5f;
    private float elapsedTimeSinceLastHit;
    private float healthRegenTimer;

    private bool healingEnabled = true;

    private void Awake()
    {
        entity = GetComponent<Entity>();
    }

    private void OnEnable()
    {
        entity.OnEntityTakeDamage += Entity_OnEntityTakeDamage;
        entity.OnEntityDealDamage += Entity_OnEntityDealDamage;
    }

    private void OnDisable()
    {
        entity.OnEntityTakeDamage -= Entity_OnEntityTakeDamage;
        entity.OnEntityDealDamage -= Entity_OnEntityDealDamage;
    }

    private void Entity_OnEntityDealDamage(Entity attacker, Entity victim, Vector3 hitPoint, int damage)
    {
        elapsedTimeSinceLastHit = 0f;
    }

    private void Entity_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject sourceObject)
    {
        elapsedTimeSinceLastHit = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            healingEnabled = !healingEnabled;
            Debug.LogWarning($"Turning OOO Healing {(healingEnabled ? "On" : "Off")}");
        }

        HandleHealthRegen();
    }

    /// <summary>
    /// Handles out of combat health regen for player.
    /// If the player has not been hit for durationSinceLastHitToRegen seconds, heal for healthRegenAmount every healthRegenRate seconds.
    /// </summary>
    private void HandleHealthRegen()
    {
        if (!healingEnabled) return; // If healing is disabled
        if (entity.CurrentState == entity.EntityDeathState) return; // If dead
        if (entity.CurrentHealth == entity.MaxHealth.GetIntValue()) return; // If full
        if (entity.MaxHealth.GetIntValue() <= 0) return; // If invincible

        if (elapsedTimeSinceLastHit > durationOutOfCombatToRegen)
        {
            healthRegenTimer += entity.LocalDeltaTime;
            if (healthRegenTimer > healthRegenRate)
            {
                healthRegenTimer = 0f;
                entity.Heal(healthRegenAmount, true);
            }
        }
        else
        {
            elapsedTimeSinceLastHit += entity.LocalDeltaTime;
            healthRegenTimer = 0f;
        }
    }
}
