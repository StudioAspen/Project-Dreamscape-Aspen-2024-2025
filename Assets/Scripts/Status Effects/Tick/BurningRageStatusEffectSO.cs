using DG.Tweening;
using KBCore.Refs;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Burning Rage Stacks")]
public class BurningRageStatusEffectSO : TickStatusEffectSO
{
    [field: Header("Burning Rage Stacks: Settings")]
    [field: SerializeField] public int MaxStacks { get; private set; } = 5;
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float combustRadius = 7.5f;
    [SerializeField] private float combustDamageMultiplierPerStack = 5f;
    [SerializeField] private float tickDamageMultiplierPerStack = 0f;
    private int currentStacks;

    private protected override void OnApply()
    {
        base.OnApply();

        currentStacks = 1;
        entity.TweenTintEntity(new Color((float)currentStacks/MaxStacks, 0f, 0f));

        entity.OnEntityDeath.AddListener(Entity_OnEntityDeath);
    }

    private protected override void OnTick()
    {
        base.OnTick();

        if(damagePerTick > 0) entity.TakeDamageWithoutState(damagePerTick, entity.GetRandomPositionOnCollider(), source);
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

    // for when a status is stacking
    public override bool Override(StatusEffectSO newStatusEffect)
    {
        if (newStatusEffect.GetType() != GetType())
        {
            Debug.LogError($"Cannot override {name} with a different status effect type.");
            return false;
        }

        Ticks = (newStatusEffect as BurningRageStatusEffectSO).Ticks;

        damagePerTick = (int)(currentStacks * tickDamageMultiplierPerStack);

        if (currentStacks + 1 >= MaxStacks) return true; // still successful, we just hit max stacks

        currentStacks++;
        entity.TweenTintEntity(new Color(((float)(currentStacks) / MaxStacks), 0, 0));

        return true;
    }

    private void Entity_OnEntityDeath(GameObject killer)
    {
        Vector3 explosionPosition = entity.GetColliderCenterPosition();

        int combustExplosionDamage = (int)(currentStacks * combustDamageMultiplierPerStack);

        // make a list and grab all entities nearby
        List<Entity> enemyList = Entity.GetEntitiesThroughAOE(explosionPosition, combustRadius);
        for (int i = 0; i < enemyList.Count; i++) // loop through all entities and filter out friendly ones
        {
            Entity enemy = enemyList[i]; // current entity in the loop

            if (enemy == entity) continue; // filter out self (entity that died)
            if (enemy.Team != entity.Team) continue; // filter out unfriendly entities

            enemy.TakeDamage(combustExplosionDamage, enemy.GetComponent<Collider>().ClosestPointOnBounds(explosionPosition), source); // deal damage to enemy entities
        }

        CreateTemporaryVisualizer(explosionPosition, combustRadius, 0.25f);
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
