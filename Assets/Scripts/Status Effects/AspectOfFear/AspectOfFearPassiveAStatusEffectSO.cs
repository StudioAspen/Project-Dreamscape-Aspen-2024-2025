using Dreamscape.Abilities;
using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Aspect of Fear Passive A", menuName = "Status Effect/Aspects/Aspect of Fear/Passive A")]
public class AspectOfFearPassiveAStatusEffectSO : StatusEffectSO
{
    private PlayerCombat playerCombat;

    [field: Header("Aspect of Fear Passive A: Settings")]
    [field: SerializeField] public GhastlyGrievanceSkull GhastlyGrievanceSkullPrefab { get; private set; }

    [field: Header("Aspect of Fear Passive A Expanded: Settings")]
    [field: SerializeField] public float DamageUpPerSkulledEntity { get; private set; } = 0f;
    private int skulledEntityCount;

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        playerCombat = entity.GetComponent<PlayerCombat>();
        if (playerCombat == null)
        {
            Debug.LogError($"{name}: Player combat not found on entity: {entity.name}");
            RemoveSelf(); // If theres no playerCombat, remove this passive
            return;
        }

        skulledEntityCount = 0;

        playerCombat.OnAttackInputSwitched += PlayerCombat_OnAttackInputSwitched;
    }

    public override void Update()
    {
        base.Update();

        // Debug
        if(Input.GetKeyDown(KeyCode.R))
        {
            GhastlyGrievanceSkull skull = ObjectPoolerManager.Instance.SpawnPooledObject<GhastlyGrievanceSkull>(GhastlyGrievanceSkullPrefab.gameObject, entity.GetColliderCenterPosition());
            skull.Init(entity);
        }
    }

    public override void Cancel()
    {
        base.Cancel();

        playerCombat.OnAttackInputSwitched -= PlayerCombat_OnAttackInputSwitched;

        entity.DamageModifier.ClearMultipliersFromSource(this);
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        AspectOfFearPassiveAStatusEffectSO overridingStatusEffect = newStatusEffect as AspectOfFearPassiveAStatusEffectSO;

        GhastlyGrievanceSkullPrefab = overridingStatusEffect.GhastlyGrievanceSkullPrefab;
    }

    private void PlayerCombat_OnAttackInputSwitched(ComboAction previousAttackAction, ComboAction newAttackAction)
    {
        Debug.Log($"Detected attack action switch from {previousAttackAction} to {newAttackAction}");

        // Launch a piercing skull that moves forward from where the player is facing
        // and applies a skull debuff to any enemies hit by the skull
        GhastlyGrievanceSkull skull = ObjectPoolerManager.Instance.SpawnPooledObject<GhastlyGrievanceSkull>(GhastlyGrievanceSkullPrefab.gameObject, entity.GetColliderCenterPosition());
        skull.Init(entity);
    }

    /// <summary>
    /// Adds to the skulled entity count.
    /// </summary>
    public void AddSkulledEntity(int count)
    {
        if (DamageUpPerSkulledEntity <= 0f) return;

        skulledEntityCount += count;
        entity.DamageModifier.ClearMultipliersFromSource(this);
        
        if(skulledEntityCount >= 0) entity.DamageModifier.AddMultiplier(1f + skulledEntityCount * DamageUpPerSkulledEntity, this);
        Debug.Log($"Skulled entity count: {skulledEntityCount}, with damage modifier: {entity.DamageModifier.GetTotalMultiplier()}");
    }
}