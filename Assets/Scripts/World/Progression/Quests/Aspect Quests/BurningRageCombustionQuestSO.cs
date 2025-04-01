using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "_x_Times_y_ByCombustionQuestSO", menuName = "World/Progression Quest/Aspect Quests/Burning Rage Combustion")]
public class BurningRageCombustionQuestSO : AspectQuestSO
{
  [field: Header("Config")]
  [field: Range(1, 5)]
  [field: SerializeField] public int SuccessfulHitsGoal { get; private set; }
  [field: SerializeField] public int SuccessfulKillsGoal { get; private set; }
  [field: SerializeField] public StatusEffectSO TargetStatusEffect { get; private set; }

  private Player player;
  private PlayerCombat playerCombat; 
  private Dictionary<Entity, BurningRageStatusEffectSO> affectedEntities = new Dictionary<Entity, BurningRageStatusEffectSO>();
  private int successfulHitsByCombustion = 0;
  private int successfulKillsByCombustion = 0;

  private protected override void OnActivated()
  {
    player = progressionManager?.player;

    if (player == null)
      return;
    
    playerCombat = player.GetComponent<PlayerCombat>();
    if (playerCombat != null && playerCombat.Weapon != null)
    {
        playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;
    }
  }

  private protected override void OnCleanUp()
  {

  }

  private protected override void OnUpdate()
  {
    // if (IsCompleted)
    //   return;

    // if (successfulHits >= SuccessfulHitsGoal)
    //   Complete();
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

  private void AffectedEntity_OnCombustionDamage(Entity source, Entity victim, int damageValue)
  {
    Debug.Log(affectedEntities.ContainsKey(source));
    Debug.Log(source.name);
    if(affectedEntities.ContainsKey(source))
    {
      affectedEntities[source].OnCombustionDamage -= AffectedEntity_OnCombustionDamage;
      affectedEntities.Remove(source);

      successfulHitsByCombustion++;
      Debug.Log($"You hit a nearby enemy with Combustion Damage!");
    }
  }
}
