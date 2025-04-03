using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Defeat_x_DreamonsWanderlustQuestSO", menuName = "World/Progression Quest/World Event Quests/Defeat Dreamons (Wanderlust)")]
public class DefeatDreamonsWanderlustQuest : WorldEventQuestSO
{
  [field: Header("Config")]
  [field: Range(1, 3)]
  [field: SerializeField] public int RequiredDefeatsPerLandGoal { get; private set; }
  private VisitAllWorldEventSO wanderlustEventRef;
  private Dictionary<LandManager, int> defeatedFromLitLandsDictionary = new Dictionary<LandManager, int>();
  private Player player;
  private PlayerCombat playerCombat;
  private bool initialized = false;

  private protected override void OnActivated()
  {
    wanderlustEventRef = (VisitAllWorldEventSO)eventManager?.CurrentEvent;
    if (wanderlustEventRef == null)
    {
      CleanUp();
      return;
    }

    foreach (LandManager land in wanderlustEventRef.highlightedLands)
      defeatedFromLitLandsDictionary.Add(land, 0);

    // This indicated that we initially found lands.
    initialized = defeatedFromLitLandsDictionary.Count > 0;

    player = progressionManager?.player;
    playerCombat = player.GetComponent<PlayerCombat>();

    if (player == null)
    {
      CleanUp();
      return;
    }
    
    if (playerCombat.Weapon != null)
      playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;
  }

  private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue) 
  {
    // If the Dreamon did not die, do not proceed to casting.
    if (victim.CurrentHealth > damageValue)
      return;

    Enemy enemy = (Enemy)victim;
    EnemySpawner spawner;

    // If the dead entity is not an enemy, do not continue.
    if(enemy == null)
      return;
    else
      spawner = enemy.Spawner;
    

    // Check if the Enemy Spawner belongs to one of the initially highlighted lands
    LandManager land = defeatedFromLitLandsDictionary.Keys.ToList().Find(x => x.EnemySpawner == spawner);
    if (land != null)
    {
      // Add 1 to the number of defeated Dreamons from that land.
      defeatedFromLitLandsDictionary[land]++;

      // If the player has defeated the required number of Dreamons, remove it from the dictionary.
      if (defeatedFromLitLandsDictionary[land] >= RequiredDefeatsPerLandGoal)
        defeatedFromLitLandsDictionary.Remove(land);
    }
  }

  private protected override void OnCleanUp()
  {
    initialized = false;
    defeatedFromLitLandsDictionary.Clear();
  }

  private protected override void OnUpdate()
  {
    if (initialized && defeatedFromLitLandsDictionary.Count == 0)
      Complete();
  }


}
