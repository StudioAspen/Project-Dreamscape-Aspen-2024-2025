using DG.Tweening;
using KBCore.Refs;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Rage/Passive A/Burning Rage Stacks")]
public class BurningRageStatusEffectSO : TickStatusEffectSO
{
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

        entity.TweenTintEntity(GetColorBasedOnStacks(currentStacks));

        entity.OnEntityDeath.AddListener(Entity_OnEntityDeath);
    }

    private protected override void OnTick()
    {
        base.OnTick();

        if(damagePerTick > 0) entity.TakeDamage(damagePerTick, entity.GetRandomPositionOnCollider(), source, false);
    }

    private protected override void OnExpire()
    {
        entity.TweenUnTintEntity();

        entity.OnEntityDeath.RemoveListener(Entity_OnEntityDeath);

        base.OnExpire();
    }

    public override void Cancel()
    {
        entity.ResetTint();

        entity.OnEntityDeath.RemoveListener(Entity_OnEntityDeath);

        base.Cancel();
    }

    public override bool OnStack(StatusEffectSO newStatusEffect)
    {
        if (newStatusEffect.GetType() != GetType())
        {
            Debug.LogError($"Cannot override {name} with a different status effect type.");
            return false;
        }

        currentTicks = 0; // reset the ticks
        TickDamageMultiplierPerStack = (newStatusEffect as BurningRageStatusEffectSO).TickDamageMultiplierPerStack; // For when we get the extended version of burning rage

        damagePerTick = CalculateDamagePerTick(currentStacks); // Recalculate damage per tick based on our multiplier

        if (currentStacks >= MaxStacks) return true; // still successful, we just hit max stacks

        currentStacks++;

        damagePerTick = CalculateDamagePerTick(currentStacks); // Calculate again based on new stacks

        entity.TweenTintEntity(GetColorBasedOnStacks(currentStacks)); // Change entity color based on new stacks

        return true;
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

            TrySpreadToNearbyAlly(enemy, source, ref hasSpreadedToNearestAlly); // try to spread to nearby ally (if not already spreaded)

            enemy.TakeDamage(combustExplosionDamage, enemy.GetComponent<Collider>().ClosestPointOnBounds(explosionPosition), source); // deal damage to enemy entities
        }

        CreateTemporaryVisualizer(explosionPosition, currentCombustRadius, 0.25f);
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
    /// <param name="killerObject">The object responsible for killing the entity.</param>
    /// <param name="hasSpreadedToNearbyAlly">A reference to a boolean indicating whether the status effect has already spread to a nearby ally.</param>
    private void TrySpreadToNearbyAlly(Entity target, GameObject killerObject, ref bool hasSpreadedToNearbyAlly)
    {
        if (hasSpreadedToNearbyAlly) return;
        hasSpreadedToNearbyAlly = true;

        if (TickDamageMultiplierPerStack == 0) return; // This isnt the extended version of the status effect

        for (int j = 0; j < currentStacks; j++) EntityStatusEffector.TryApplyStatusEffect(target.gameObject, this, killerObject);
    }

    private void CreateTemporaryVisualizer(Vector3 hitPoint, float radius, float expireDuration)
    {
        // creates a sphere of the explosion radius
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = hitPoint;
        sphere.GetComponent<Collider>().isTrigger = true;
        sphere.transform.localScale = radius * Vector3.one;
        SetTransparent(sphere.GetComponent<Renderer>().material);
        sphere.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        sphere.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.2f);
        Destroy(sphere, expireDuration);
    }

    private void SetTransparent(Material targetMaterial)
    {
        if (targetMaterial == null) return;

        targetMaterial.shader = Shader.Find("Universal Render Pipeline/Unlit");

        // Change Surface Type to Transparent
        targetMaterial.SetFloat("_Surface", 1); // 1 = Transparent, 0 = Opaque

        // Enable required shader keywords
        targetMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        targetMaterial.DisableKeyword("_SURFACE_TYPE_OPAQUE");

        // Set rendering mode for transparency
        targetMaterial.SetOverrideTag("RenderType", "Transparent");
        targetMaterial.SetInt("_ZWrite", 0); // Disable ZWrite for transparency
        targetMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        targetMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        // Apply the changes to the material
        targetMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}
