using Dreamscape.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Aspect of Fear Passive A", menuName = "Status Effect/Aspects/Aspect of Fear/Passive A")]
public class AspectOfFearPassiveAStatusEffectSO : StatusEffectSO
{
    private Player player;
    private PlayerInputReader playerInputReader;

    [field: Header("Aspect of Fear Passive A: Settings")]
    [field: SerializeField] public GhastlyGrievanceSkull GhastlyGrievanceSkullPrefab { get; private set; }
    [field: SerializeField] public float SwitchInputDetectionMaxDuration { get; private set; } = 0.5f;
    private float switchInputDetectionTimer;
    private ComboAction previousInput;

    [field: Header("Aspect of Fear Passive A Expanded: Settings")]
    [field: SerializeField] public float DamageUpPerSkulledEntity { get; private set; } = 0f;
    public Dictionary<Entity, ExtendedDebuffStatusEffectSO> skulledEntities = new Dictionary<Entity, ExtendedDebuffStatusEffectSO>();
    /// <summary>
    /// An action that is invoked when the entity is executed and safely removed from the skulled entities dictionary.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>ExtendedDebuffStatusEffectSO Ghastly Grievance</c>: The debuff attached to the executed entity.</description></item>
    /// </list>
    /// </remarks>
    public Action<ExtendedDebuffStatusEffectSO> OnSkulledEntityExecuted;

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        player = entity as Player;
        if (player == null)
        {
            Debug.LogError($"{name}: Not a player: {entity.name}");
            RemoveSelf(); // If theres no playerCombat, remove this passive
            return;
        }

        playerInputReader = player.GetComponent<PlayerInputReader>();
        if (playerInputReader == null)
        {
            Debug.LogError($"{name}: playerInputReader not found on entity: {entity.name}");
            RemoveSelf(); // If theres no playerCombat, remove this passive
            return;
        }

        skulledEntities.Clear();

        playerInputReader.OnComboAction += PlayerInputReader_OnComboAction;
    }

    public override void Update()
    {
        base.Update();

        HandleSwitchInputTimer();
    }

    public override void Cancel()
    {
        base.Cancel();

        playerInputReader.OnComboAction -= PlayerInputReader_OnComboAction;

        entity.DamageModifier.ClearMultipliersFromSource(this);
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        AspectOfFearPassiveAStatusEffectSO overridingStatusEffect = newStatusEffect as AspectOfFearPassiveAStatusEffectSO;

        GhastlyGrievanceSkullPrefab = overridingStatusEffect.GhastlyGrievanceSkullPrefab;
    }

    private void PlayerInputReader_OnComboAction(ComboAction newAction)
    {
        if (switchInputDetectionTimer > SwitchInputDetectionMaxDuration)
        {
            switchInputDetectionTimer = 0;
            return;
        }

        if(previousInput == ComboAction.ATTACK1 && newAction == ComboAction.ATTACK2)
        {
            OnAttackInputSwitched();
        }
        else if(previousInput == ComboAction.ATTACK2 && newAction == ComboAction.ATTACK1)
        {
            OnAttackInputSwitched();
        }

        switchInputDetectionTimer = 0;
        previousInput = newAction;
    }

    private void HandleSwitchInputTimer()
    {
        if (player.CurrentState == player.PlayerAttackState) return;
        if (switchInputDetectionTimer > SwitchInputDetectionMaxDuration) return;
        switchInputDetectionTimer += Time.deltaTime;
    }

    private void OnAttackInputSwitched()
    {
        // Launch a piercing skull that moves forward from where the player is facing
        // and applies a skull debuff to any enemies hit by the skull
        GhastlyGrievanceSkull skull = ObjectPoolerManager.Instance.SpawnPooledObject<GhastlyGrievanceSkull>(GhastlyGrievanceSkullPrefab.gameObject, entity.GetColliderCenterPosition());
        skull.Init(entity);
    }

    /// <summary>
    /// Adds to the skulled entities dictionary.
    /// </summary>
    public void AddSkulledEntity(Entity newSkulledEntity, ExtendedDebuffStatusEffectSO ghastlyGrievance)
    {
        if(skulledEntities.ContainsKey(newSkulledEntity))
          return;
          
        skulledEntities.Add(newSkulledEntity, ghastlyGrievance);
        ghastlyGrievance.OnExecution += RemoveSkulledEntity;
        
        if (DamageUpPerSkulledEntity <= 0f) 
          return;

        entity.DamageModifier.ClearMultipliersFromSource(this);
        
        if(skulledEntities.Count >= 0) 
          entity.DamageModifier.AddMultiplier(1f + skulledEntities.Count * DamageUpPerSkulledEntity, this);

        Debug.Log($"Skulled entities: {skulledEntities.Count}, with damage modifier: {entity.DamageModifier.GetTotalMultiplier()}");
    }

    /// <summary>
    /// Removes from the skulled entities dictionary.
    /// </summary>
    public void RemoveSkulledEntity(Entity skulledEntity)
    {
        if (!skulledEntities.ContainsKey(skulledEntity))
          return;

        skulledEntities[skulledEntity].OnExecution = null;
        OnSkulledEntityExecuted?.Invoke(skulledEntities[skulledEntity]);
        skulledEntities.Remove(skulledEntity);
        
        if (DamageUpPerSkulledEntity <= 0f) 
          return;

        entity.DamageModifier.ClearMultipliersFromSource(this);

        if(skulledEntities.Count >= 0) 
          entity.DamageModifier.AddMultiplier(1f + skulledEntities.Count * DamageUpPerSkulledEntity, this);

        Debug.Log($"Skulled entities: {skulledEntities.Count}, with damage modifier: {entity.DamageModifier.GetTotalMultiplier()}");
    }
}