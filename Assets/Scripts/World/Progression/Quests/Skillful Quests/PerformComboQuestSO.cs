using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_Times_y_z_ComboQuestSO", menuName = "World/Progression Quest/Skillful Quests/Combo")]
public class PerformComboQuestSO : SkillfulQuestSO
{
  [field: Header("Config")]
  [field: Range(0, 5)]
  [field: SerializeField] public int SuccessfulPerformancesGoal { get; private set; }
  [field: Range(0, 5)]
  [field: SerializeField] public int SuccessfulHitsGoal { get; private set; }
  [field: SerializeField] public ComboDataSO TargetComboData { get; private set; } 

  private Player player;
  private PlayerCombat playerCombat; 
  private PlayerAttackState playerAttackState;
  private int successfulPerformances = 0;
  private int successfulHits = 0;

  private protected override void OnActivated()
  {
    player = progressionManager?.player;
    playerCombat = player.GetComponent<PlayerCombat>();
    playerAttackState = player.PlayerAttackState;

    if (player == null)
    {
      CleanUp();
      return;
    }
    
    if (playerCombat.Weapon != null)
    {
      playerCombat.Weapon.OnWeaponStartSwing += PlayerWeapon_OnWeaponStartSwing;
      playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;
    }
  }


  private protected override void OnCleanUp()
  {
    if (playerCombat.Weapon != null)
    {
      playerCombat.Weapon.OnWeaponStartSwing -= PlayerWeapon_OnWeaponStartSwing;
      playerCombat.Weapon.OnWeaponHit -= PlayerWeapon_OnWeaponHit;
    }
  }

  private protected override void OnUpdate()
  {
    if (successfulPerformances >= SuccessfulPerformancesGoal && successfulHits >= SuccessfulHitsGoal)
      Complete();
  }

  private void PlayerWeapon_OnWeaponStartSwing(Entity entity, ComboDataSO sO)
  {
    if (player.CurrentState != playerAttackState || playerAttackState.ComboData == null)
      return;

    if (playerAttackState.ComboData.DisplayName == TargetComboData.DisplayName)
      successfulPerformances++;
  }

  private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue) 
  {
    if (player.CurrentState != playerAttackState || playerAttackState.ComboData == null)
      return;

    if (playerAttackState.ComboData.DisplayName == TargetComboData.DisplayName)
      successfulHits++;
  }
}
