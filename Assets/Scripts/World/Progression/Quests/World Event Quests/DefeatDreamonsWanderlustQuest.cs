using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Defeat_x_DreamonsWanderlustQuestSO", menuName = "World/Progression Quest/World Event Quests/Defeat Dreamons (Wanderlust)")]
public class DefeatDreamonsWanderlustQuest : WorldEventQuestSO
{
  [field: Header("Defeat Dreamons (Wanderlust) Configuration")]

  /// <summary>
  /// The number of Dreamons players must defeat from each highlighted Land to complete the quest.
  /// </summary>
  [Tooltip("The number of Dreamons players must defeat from each highlighted Land to complete the quest.")]
  [field: Range(1, 3)]
  [field: SerializeField] public int DefeatsPerLandGoal { get; private set; }

  /// <summary>
  /// Dictionary that tracks the number of defeated Dreamons from each highlighted Land during the quest.
  /// </summary>
  /// <typeparam name="LandManager">The highlighted Land</typeparam>
  /// <typeparam name="int">The number of defeated Dreamons from the highlighted Land.</typeparam>
  private Dictionary<LandManager, int> defeatedFromLandDictionary = new Dictionary<LandManager, int>();

  /// <summary>
  /// Reference to the Player via the Progression Manager.
  /// </summary>
  private Player player;

  /// <summary>
  /// Reference to the Player Combat via the Progression Manager and Player.
  /// </summary>
  private PlayerCombat playerCombat; 

  /// <summary>
  /// Reference to the Wanderlust World Event via the Event Manager.
  /// </summary>
  private VisitAllWorldEventSO wanderlustEvent;

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
    else if (!(VisitAllWorldEventSO)eventManager.CurrentEvent)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Required World Event is not of type {new VisitAllWorldEventSO().GetType()}.");

      return false;  
    }

    // Assign the references to the corresponding variables.
    playerCombat ??= playerCombatRef;
    wanderlustEvent ??= (VisitAllWorldEventSO)eventManager?.CurrentEvent;

    if (!playerCombat.Weapon)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the player Weapon.");

      return false;
    }

    return base.MeetsCriteria(progressionManager);
  }

  private protected override void OnActivated()
  {
    foreach (LandManager land in wanderlustEvent.highlightedLands)
      defeatedFromLandDictionary.Add(land, 0);

    // Subscribe to the necessary Actions.
    playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;
  }

  // Unsubscribe to any Actions used for the quest.
  private protected override void OnCleanUp() => playerCombat.Weapon.OnWeaponHit -= PlayerWeapon_OnWeaponHit;

  private protected override void OnUpdate() { }

  private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue) 
  {
    // If the Dreamon did not die, do not proceed to casting.
    if (victim.CurrentHealth > damageValue)
      return;

    // If the dead entity is not an enemy, do not continue.
    if(!(Enemy)victim)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} On Weapon Hit Error: Victim entity is not of type {new Enemy().GetType()}.");
 
      return;
    }
    
    // Assign the references to the corresponding variables.
    Enemy enemy = (Enemy)victim;
    EnemySpawner spawner = enemy.Spawner;
    
    // Check if the Enemy Spawner belongs to one of the initially highlighted lands
    LandManager land = defeatedFromLandDictionary.Keys.ToList().Find(x => x.EnemySpawner == spawner);
    if (land)
    {
      // Add 1 to the number of defeated Dreamons from that land.
      defeatedFromLandDictionary[land]++;

      // If the player has defeated the required number of Dreamons, remove it from the dictionary.
      if (defeatedFromLandDictionary[land] >= DefeatsPerLandGoal)
        defeatedFromLandDictionary.Remove(land);
    }

    // Check if there are no more items in the dictionary.
    if (defeatedFromLandDictionary.Count == 0)
      Complete();
  }




}
