using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Rage/Passive A/Burning Rage Stacks")]
public class BurningRageStatusEffectSO : TickStatusEffectSO
{
    private EntityRendererManager entityRendererManager;

    [field: Header("Burning Rage Stacks: Settings")]
    [field: SerializeField] public int MaxStacks { get; private set; } = 5;
    [SerializeField] private float defaultCombustRadius = 7.5f;
    [SerializeField] private float combustRadiusMultipler = 1.1f;
    [SerializeField] private float combustDamageMultiplierPerStack = 5f;
    [field: SerializeField] public float TickDamageMultiplierPerStack { get; private set; } = 0f;
    private int damagePerTick;
    private int currentStacks;

    private protected override void OnApply()
    {
        base.OnApply();

        currentStacks = 1;

        damagePerTick = CalculateDamagePerTick(currentStacks); // Calculate initial damage per tick

        entityRendererManager = entity.GetComponent<EntityRendererManager>();
        if (entityRendererManager) entityRendererManager.TweenTint(GetColorBasedOnStacks(currentStacks));

        entity.OnEntityDeath += Entity_OnEntityDeath;
    }

    private protected override void OnTick()
    {
        base.OnTick();

        if(damagePerTick > 0) entity.TakeDamage(damagePerTick, entity.GetRandomPositionOnCollider(), source, false); // Don't use DealDamageToEntity as we don't want DOT to count as lifesteal
    }

    private protected override void OnExpire()
    {
        if (entityRendererManager) entityRendererManager.TweenUnTint();

        entity.OnEntityDeath -= Entity_OnEntityDeath;

        base.OnExpire();
    }

    public override void Cancel()
    {
        if (entityRendererManager) entityRendererManager.ResetTint();

        entity.OnEntityDeath -= Entity_OnEntityDeath;

        base.Cancel();
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        base.OnStack(newStatusEffect);

        BurningRageStatusEffectSO overridingStatusEffect = newStatusEffect as BurningRageStatusEffectSO;

        Ticks = overridingStatusEffect.Ticks; // So the total number of ticks doesn't skyrocket to a high number. Default is +=.

        currentTicks = 0; // reset the ticks
        TickDamageMultiplierPerStack = overridingStatusEffect.TickDamageMultiplierPerStack; // For when we get the extended version of burning rage

        damagePerTick = CalculateDamagePerTick(currentStacks); // Recalculate damage per tick based on our multiplier

        if (currentStacks >= MaxStacks) return; // we hit max stacks

        currentStacks++;

        damagePerTick = CalculateDamagePerTick(currentStacks); // Calculate again based on new stacks

        if (entityRendererManager) entityRendererManager.TweenTint(GetColorBasedOnStacks(currentStacks)); // Change entity color based on new stacks
    }

    private void Entity_OnEntityDeath(GameObject killer)
    {
        Vector3 explosionPosition = entity.GetColliderCenterPosition();

        int combustExplosionDamage = (int)(currentStacks * combustDamageMultiplierPerStack);
        float currentCombustRadius = Mathf.Pow(combustRadiusMultipler, currentStacks) * defaultCombustRadius;

        bool hasSpreadedToNearestAlly = false;

        // make a list and grab all non-dead entities nearby
        List<Entity> enemyList = Entity.GetEntitiesThroughAOE(explosionPosition, currentCombustRadius, false);
        for (int i = 0; i < enemyList.Count; i++) // loop through all entities and filter out friendly ones
        {
            Entity enemy = enemyList[i]; // current entity in the loop

            if (enemy == entity) continue; // filter out self (entity that died)

            if (enemy.Team != entity.Team) continue; // filter out unfriendly entities

            TrySpreadToNearbyAlly(enemy, ref hasSpreadedToNearestAlly); // try to spread to nearby ally (if not already spreaded)

            if(source.TryGetComponent(out Entity sourceEntity))
            {
                sourceEntity.DealDamageToOtherEntity(enemy, combustExplosionDamage, enemy.CharacterController.ClosestPointOnBounds(explosionPosition));
            }
            else
            {
                enemy.TakeDamage(combustExplosionDamage, enemy.CharacterController.ClosestPointOnBounds(explosionPosition), source); // deal damage to enemy entities
            }
        }

        CustomDebug.InstantiateTemporarySphere(explosionPosition, currentCombustRadius, 0.25f, new Color(1f, 0, 0, 0.2f));
    }

    /// <summary>
    /// Returns the color based on the number of stacks.
    /// </summary>
    /// <param name="stacks">The number of stacks.</param>
    /// <returns>The color based on the number of stacks.</returns>
    private Color GetColorBasedOnStacks(int stacks)
    {
        return new Color((float)stacks / MaxStacks, 0f, 0f);
    }

    /// <summary>
    /// Calculates the damage per tick based on the number of stacks.
    /// </summary>
    /// <param name="stacks">The number of stacks.</param>
    /// <returns>The damage per tick based on the number of stacks.</returns>
    private int CalculateDamagePerTick(int stacks)
    {
        return (int)(stacks * TickDamageMultiplierPerStack);
    }

    /// <summary>
    /// Tries to spread the status effect to a nearby ally entity.
    /// </summary>
    /// <param name="target">The target entity to spread the status effect to.</param>
    /// <param name="hasSpreadedToNearbyAlly">A reference to a boolean indicating whether the status effect has already spread to a nearby ally.</param>
    private void TrySpreadToNearbyAlly(Entity target, ref bool hasSpreadedToNearbyAlly)
    {
        if (hasSpreadedToNearbyAlly) return;
        hasSpreadedToNearbyAlly = true;

        if (TickDamageMultiplierPerStack == 0) return; // This isnt the extended version of the status effect

        // repeatedly apply the status effect to the target entity based on the number of stacks
        for (int j = 0; j < currentStacks; j++) EntityStatusEffector.TryApplyStatusEffect(target.gameObject, this, source);
    }
}
