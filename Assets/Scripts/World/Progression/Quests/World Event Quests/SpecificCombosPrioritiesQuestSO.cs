using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_CombosOnlyPrioritiesQuestSO", menuName = "World/Progression Quest/World Event Quests/Specific Combos (Priorities)")]
public class SpecificCombosPrioritiesQuestSO : WorldEventQuestSO
{
  [field: Header("Specific Combos (Priorities) Configuration")]

  /// <summary>
  /// List of the combos the player is allowed to hit marked Dreamons with during the quest.
  /// </summary>
  [field: Tooltip("List of the combos the player is allowed to hit marked Dreamons with during the quest.")]
  [field: SerializeField] List<ComboDataSO> allowedCombos;

  /// <summary>
  /// Did the player hit a marked Dreamon with a combo not listed in the allowed Combos list?
  /// </summary>
  private bool landedProhibitedCombo = false;

  /// <summary>
  /// Reference to the Player via the Progression Manager.
  /// </summary>
  private Player player;

  /// <summary>
  /// Reference to the Player Combat via the Progression Manager and Player.
  /// </summary>
  private PlayerCombat playerCombat; 

  /// <summary>
  /// Reference to the Player Attack State via the Progression Manager and Player.
  /// </summary>
  private PlayerAttackState playerAttackState;

  /// <summary>
  /// Reference to the Defend World Event via the Event Manager.
  /// </summary>
  private PrioritiesWorldEventSO prioritiesEvent;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    if (!progressionManager.player)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the player.");

      return false;
    }
    else if (!progressionManager.eventManager)
    {
      if (LogErrorMessages) 
        Debug.LogError($"{name} Criteria Error: Could not find reference to the Event Manager.");

      return false;
    }
    else if (allowedCombos.Count == 0)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: A list of allowed Combos was not provided.");

      return false;
    }
    
    // Assign the references to the corresponding variables.
    player ??= progressionManager.player;
    eventManager ??= progressionManager.eventManager;

    if (!progressionManager.player.TryGetComponent(out PlayerCombat playerCombatRef))
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find Player Combat component on the player.");

      return false;      
    }
    else if (player.PlayerAttackState == null)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the player Attack State.");

      return false;   
    }
    else if (eventManager.CurrentEvent is not PrioritiesWorldEventSO)
    {
      {
        if (LogErrorMessages)
          Debug.LogError($"{name} Criteria Error: Required World Event is not of type {typeof(PrioritiesWorldEventSO)}.");

        return false;  
      }
    }

    // Assign the references to the corresponding variables.
    playerCombat ??= playerCombatRef;
    playerAttackState ??= player.PlayerAttackState;
    prioritiesEvent ??= (PrioritiesWorldEventSO)eventManager.CurrentEvent;

    if (!playerCombat.Weapon)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the player Weapon.");

      return false;
    }

    return true;
  }

  private protected override void OnActivated() => playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;
  private protected override void OnCleanUp()
  {
    // Unsubscribe to any Actions used for the quest.
    playerCombat.Weapon.OnWeaponHit -= PlayerWeapon_OnWeaponHit;

    // Check for completion at the end of the wave.
    if (landedProhibitedCombo == false)
      Complete(false);
  }

  private protected override void OnUpdate() { }

  private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue) 
  {
    if (player.CurrentState != playerAttackState)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} On Weapon Hit Error: Player's current State is not the Attack State.");

      return;
    }
    else if (!playerAttackState.ComboData)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} On Weapon Hit Error: Could not find a Combo Data reference on the player Attack State.");

      return;
    }
    // Ignore the following checks if the Dreamon is still alive
    else if (victim.CurrentHealth > damageValue)
      return;
    // Ignore the next check if the Dreamon is not marked by this event.
    else if (!prioritiesEvent.enemyMarkers.ContainsKey(victim as Enemy))
      return;

    // If the combo is not in the allowed combos list, the player fails this event
    if (allowedCombos.Find(c => c.DisplayName != playerAttackState.ComboData.DisplayName))
      landedProhibitedCombo = true;
  }

}
