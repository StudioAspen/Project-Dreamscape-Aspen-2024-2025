using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_Times_y_ByIntimidatingSlamQuestSO", menuName = "World/Progression Quest/Aspect Quests/Intimidating Slam")]
public class IntimidatingSlamQuestSO : AspectQuestSO
{
  [field: Header("Config")]
  [field: Range(0, 5)]
  [field: SerializeField] public int SuccessfulHitsGoal { get; private set; }
  [field: Range(0, 5)]
  [field: SerializeField] public int SuccessfulKillsGoal { get; private set; }
  [field: SerializeField] public ComboDataSO TargetComboData { get; private set; }

  private Player player;
  private PlayerCombat playerCombat; 
  private PlayerAttackState playerAttackState;
  private int successfulHitsByIntimidatingSlam = 0;
    private int successfulKillsByIntimidatingSlam = 0;

  private protected override void OnActivated()
  {
    player = progressionManager?.player;
    playerCombat = player.GetComponent<PlayerCombat>();
    playerAttackState = player.PlayerAttackState;

    if (player == null)
      CleanUp();
    
    if (playerCombat.Weapon != null)
      playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;
  }


  private protected override void OnCleanUp()
  {
    if (playerCombat.Weapon != null)
      playerCombat.Weapon.OnWeaponHit -= PlayerWeapon_OnWeaponHit;
  }

  private protected override void OnUpdate()
  {
    if (successfulHitsByIntimidatingSlam >= SuccessfulHitsGoal && successfulKillsByIntimidatingSlam >= SuccessfulKillsGoal)
      Complete();
  }

  private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue)
  {
    if (player.CurrentState != playerAttackState || playerAttackState.ComboData == null)
      return;

    if (playerAttackState.ComboData.DisplayName == TargetComboData.DisplayName)
    {
      successfulHitsByIntimidatingSlam++;
      if (victim.CurrentHealth <= damageValue)
        successfulKillsByIntimidatingSlam++;
    }
  }
}
