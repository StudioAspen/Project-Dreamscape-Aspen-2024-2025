using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_CombosOnlyPrioritiesQuestSO", menuName = "World/Progression Quest/World Event Quests/Specific Combos Only (Priorities)")]
public class SpecificCombosPrioritiesQuestSO : WorldEventQuestSO
{
  [field: Header("Config")]
  [SerializeField] List<ComboDataSO> allowedCombos;
  private PrioritiesWorldEventSO prioritiesEventRef;
  private Player player;
  private PlayerCombat playerCombat;
  private PlayerAttackState playerAttackState;
  private bool landedProhibitedCombo = false;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    if (allowedCombos.Count == 0)
      return false;

    return base.MeetsCriteria(progressionManager);
  }

  private protected override void OnActivated()
  {
    player = progressionManager?.player;
    playerCombat = player.GetComponent<PlayerCombat>();
    playerAttackState = player.PlayerAttackState;
    prioritiesEventRef = (PrioritiesWorldEventSO)eventManager?.CurrentEvent;

    if (player == null || prioritiesEventRef == null)
    {
      landedProhibitedCombo = true;
      CleanUp();
      return;
    }

    landedProhibitedCombo = false;

    if (playerCombat.Weapon != null || playerAttackState != null)
      playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;
  }

  private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue) 
  {
    if (player.CurrentState != playerAttackState || playerAttackState.ComboData == null)
      return;

    // Ignore the following checks if the Dreamon is still alive
    if (victim.CurrentHealth > damageValue)
      return;

    // Ignore the next check if the Dreamon is not marked by this event.
    if (!prioritiesEventRef.enemyMarkers.ContainsKey(victim as Enemy))
      return;

    // If the combo is not in the allowed combos list, the player fails this event
    if (allowedCombos.Find(c => c.DisplayName != playerAttackState.ComboData.DisplayName))
      landedProhibitedCombo = true;
  }

  private protected override void OnCleanUp()
  {
    if (playerCombat.Weapon != null || playerAttackState != null)
      playerCombat.Weapon.OnWeaponHit -= PlayerWeapon_OnWeaponHit;

    if (landedProhibitedCombo == false)
      Complete(false);
  }

  private protected override void OnUpdate()
  {
    Debug.Log($"Player defeated a marked Dreamon with a prohibited combo: {landedProhibitedCombo}");
  }
}
