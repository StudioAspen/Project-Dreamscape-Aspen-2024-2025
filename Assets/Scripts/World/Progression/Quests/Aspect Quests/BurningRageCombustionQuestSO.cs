using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "_x_Times_y_ByCombustionQuestSO", menuName = "World/Progression Quest/Aspect Quests/Burning Rage Combustion")]
public class BurningRageCombustionQuestSO : AspectQuestSO
{
  [field: Header("Config")]
  [field: Range(0, 5)]
  [field: SerializeField] public int SuccessfulHitsGoal { get; private set; }
  [field: Range(0, 5)]
  [field: SerializeField] public int SuccessfulKillsGoal { get; private set; }

  private Player player;
  private PlayerCombat playerCombat; 
  private Dictionary<Entity, BurningRageStatusEffectSO> affectedEntities = new Dictionary<Entity, BurningRageStatusEffectSO>();
  private int successfulHitsByCombustion = 0;
  private int successfulKillsByCombustion = 0;

  private protected override void OnActivated()
  {
    successfulHitsByCombustion = 0;
    successfulKillsByCombustion = 0;
    player = progressionManager?.player;

    if (player == null)
      CleanUp();
    
    playerCombat = player.GetComponent<PlayerCombat>();
    if (playerCombat != null && playerCombat.Weapon != null)
    {
      playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;
    }
  }

  private protected override void OnCleanUp()
  {
    if (playerCombat != null && playerCombat.Weapon != null)
      playerCombat.Weapon.OnWeaponHit -= PlayerWeapon_OnWeaponHit;

    foreach (Entity entity in affectedEntities.Keys)
      affectedEntities[entity].OnCombustionDamage -= AffectedEntity_OnCombustionDamage;

    affectedEntities.Clear();
  }

  private protected override void OnUpdate()
  {
    Debug.Log($"Successful Hits {successfulHitsByCombustion}, Successful Kills: {successfulKillsByCombustion}");

    if (successfulHitsByCombustion >= SuccessfulHitsGoal && successfulKillsByCombustion >= SuccessfulKillsGoal)
      Complete();
  }

  private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue) => CheckForBurningRage(victim);

  private void CheckForBurningRage(Entity entity)
  {
    BurningRageStatusEffectSO burningRage = EntityStatusEffector.TryGetStatusEffect<BurningRageStatusEffectSO>(entity.gameObject);

    if (burningRage != null && !affectedEntities.ContainsKey(entity))
    {
      Debug.Log($"Burning Rage Afflicted to {entity.name}!");
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

      Debug.Log($"You hit a nearby enemy with Combustion Damage!");
      successfulHitsByCombustion++;

      if (victim.CurrentHealth <= damageValue)
        successfulKillsByCombustion++;
    }
  }
}
