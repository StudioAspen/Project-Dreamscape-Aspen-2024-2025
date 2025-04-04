using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_Times_y_z_ComboQuestSO", menuName = "World/Progression Quest/Skillful Quests/Hammer Combo")]
public class HammerComboQuestSO : SkillfulQuestSO
{
  [field: Header("Hammer Combo Configuration")]

  /// <summary>
  /// The number of times the player must perform the target Combo to complete the quest.
  /// </summary>
  [field: Tooltip("The number of times the player must perform the target Combo to complete the quest.")]
  [field: Range(0, 5)]
  [field: SerializeField] public int SuccessfulPerformancesGoal { get; private set; }

  /// <summary>
  /// The number of times the player must hit Dreamons with the target Combo to complete the quest.
  /// </summary>
  [field: Tooltip("The number of times the player must hit Dreamons with the target Combo to complete the quest.")]
  [field: Range(0, 5)]
  [field: SerializeField] public int SuccessfulHitsGoal { get; private set; }

  /// <summary>
  /// The Combo used to determine the completion of the quest.
  /// </summary>
  [field: SerializeField] public ComboDataSO TargetComboData { get; private set; } 

  /// <summary>
  /// The number of times the player has performed the target Combo.
  /// </summary>
  private int successfulPerformances = 0;

  /// <summary>
  /// The number of times the player has hit Dreamons the target Combo.
  /// </summary>
  private int successfulHits = 0;

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
    else if (player.PlayerAttackState == null)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the player Attack State.");

      return false;   
    }

    // Assign the references to the corresponding variables.
    playerCombat ??= playerCombatRef;
    playerAttackState ??= player.PlayerAttackState;

    if (!playerCombat.Weapon)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the player Weapon.");

      return false;
    }

    // Check the base criteria.
    return base.MeetsCriteria(progressionManager);
  }

  private protected override void OnActivated()
  {
    // Subscribe to the necessary Actions.
    playerCombat.Weapon.OnWeaponStartSwing += PlayerWeapon_OnWeaponStartSwing;
    playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;
  }


  private protected override void OnCleanUp()
  {
    // Unsubscribe to any Actions used for the quest.
    playerCombat.Weapon.OnWeaponStartSwing -= PlayerWeapon_OnWeaponStartSwing;
    playerCombat.Weapon.OnWeaponHit -= PlayerWeapon_OnWeaponHit;
  }

  private protected override void OnUpdate() { }

  private void PlayerWeapon_OnWeaponStartSwing(Entity entity, ComboDataSO sO)
  {
    if (player.CurrentState != playerAttackState || !playerAttackState.ComboData)
      return;

    if (playerAttackState.ComboData.DisplayName == TargetComboData.DisplayName)
      successfulPerformances++;

    if(successfulPerformances >= SuccessfulPerformancesGoal && successfulHits >= SuccessfulHitsGoal)
      Complete();
  }

  private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue) 
  {
    if (player.CurrentState != playerAttackState || !playerAttackState.ComboData)
      return;

    if (playerAttackState.ComboData.DisplayName == TargetComboData.DisplayName)
      successfulHits++;

    if(successfulPerformances >= SuccessfulPerformancesGoal && successfulHits >= SuccessfulHitsGoal)
      Complete();
  }
}
