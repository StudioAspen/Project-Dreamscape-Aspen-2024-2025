using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_Times_y_ByCombustionQuestSO", menuName = "World/Progression Quest/Aspect Quests/Burning Rage Combustion")]
public class BurningRageCombustionQuestSO : AspectQuestSO
{
  [field: Header("Burning Rage Combustion Configuration")]

  /// <summary>
  /// The number of times the player must afflict Dreamons with the Burning Rage status effect to complete the quest.
  /// </summary>
  [Tooltip("The number of times the player must afflict Dreamons with the Burning Rage status effect to complete the quest.")]
  [field: Range(0, 5)]
  [field: SerializeField] public int SuccessfulHitsGoal { get; private set; }

  /// <summary>
  /// The number of times the player must defeat Dreamons with Burning Rage combustion damage to complete the quest.
  /// </summary>
  [Tooltip("The number of times the player must defeat Dreamons with Burning Rage combustion damage to complete the quest.")]
  [field: Range(0, 5)]
  [field: SerializeField] public int SuccessfulDefeatsGoal { get; private set; }

  /// <summary>
  /// The number of times the player has afflicted Dreamons with the Burning Rage status effect.
  /// </summary>
  private int successfulHits = 0;

  /// <summary>
  /// The number of times the player has defeated Dreamons with Burning Rage combustion damage.
  /// </summary>
  private int successfulDefeats = 0;

  /// <summary>
  /// Reference to the Player via the Progression Manager.
  /// </summary>
  private Player player;

  /// <summary>
  /// Reference to the Player Combat via the Progression Manager and Player.
  /// </summary>
  private PlayerCombat playerCombat;

  /// <summary>
  /// Dictionary that tracks every entity the player afflicts with the Burning Rage status effect during the quest.
  /// </summary>
  /// <typeparam name="Entity">The affected entity.</typeparam>
  /// <typeparam name="BurningRageStatusEffectSO">The entity's Burning Rage status effect.</typeparam>
  /// <returns>A dictionary with Entity keys and Burning Rage status effect values.</returns>
  private Dictionary<Entity, BurningRageStatusEffectSO> affectedEntities = new Dictionary<Entity, BurningRageStatusEffectSO>();

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    if (!progressionManager.player)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the player.");

      return false;
    }

    player ??= progressionManager.player;

    if (!progressionManager.player.TryGetComponent(out PlayerCombat playerCombatRef))
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find Player Combat component on the player.");

      return false;      
    }

    // Assign the references to the corresponding variables.
    playerCombat ??= playerCombatRef;

    if (!playerCombat.Weapon)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the player Weapon.");

      return false;
    }

    // Check the base criteria.
    return base.MeetsCriteria(progressionManager);
  }

  // Subscribe to the necessary Actions.
  private protected override void OnActivated() => playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;

  // Unsubscribe to any Actions used for the quest.
  private protected override void OnCleanUp()
  {
    playerCombat.Weapon.OnWeaponHit -= PlayerWeapon_OnWeaponHit;

    foreach (Entity entity in affectedEntities.Keys)
      affectedEntities[entity].OnCombustionDamage -= AffectedEntity_OnCombustionDamage;
  }

  private protected override void OnUpdate() { }

  private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue) => CheckForBurningRage(victim);

  private void CheckForBurningRage(Entity entity)
  {
    BurningRageStatusEffectSO burningRage = EntityStatusEffector.TryGetStatusEffect<BurningRageStatusEffectSO>(entity.gameObject);

    if (!burningRage && !affectedEntities.ContainsKey(entity))
    {
      affectedEntities.Add(entity, burningRage);
      burningRage.OnCombustionDamage += AffectedEntity_OnCombustionDamage;
    }
  }

  private void AffectedEntity_OnCombustionDamage(Entity combustionSource, Entity victim, int damageValue)
  {
    if(affectedEntities.ContainsKey(combustionSource))
    {
      affectedEntities[combustionSource].OnCombustionDamage -= AffectedEntity_OnCombustionDamage;
      affectedEntities.Remove(combustionSource);

      successfulHits++;

      if (victim.CurrentHealth <= damageValue)
        successfulDefeats++;
    }

    // Check if the updated successful hits and defeats meet the goals.
    if (successfulHits >= SuccessfulHitsGoal && successfulDefeats >= SuccessfulDefeatsGoal)
      Complete();
  }
}
