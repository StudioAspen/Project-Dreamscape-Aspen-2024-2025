using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_Times_y_ByIntimidatingSlamQuestSO", menuName = "World/Progression Quest/Aspect Quests/Intimidating Slam")]
public class IntimidatingSlamQuestSO : AspectQuestSO
{
  [field: Header("Intimidating Slam Configuration")]

  /// <summary>
  /// The number of times the player must hit Dreamons with Intimidating Slam to complete the quest.
  /// </summary>
  [field: Tooltip("The number of times the player must hit Dreamons with Intimidating Slam to complete the quest.")]
  [field: Range(0, 10)]
  [field: SerializeField] public int SuccessfulHitsGoal { get; private set; }

  /// <summary>
  /// The number of times the player must defeat Dreamons with Intimidating Slam to complete the quest.
  /// </summary>
  [field: Tooltip("The number of times the player must defeat Dreamons with Intimidating Slam to complete the quest.")]
  [field: Range(0, 10)]
  [field: SerializeField] public int SuccessfulDefeatsGoal { get; private set; }

  /// <summary>
  /// The display name of the Intimidating Slam Combo Data.
  /// </summary>
  private const string DISPLAY_NAME = "Intimidating Slam";

  /// <summary>
  /// The number of times the player has hit Dreamons with Intimidating Slam.
  /// </summary>
  private int successfulHits = 0;

  /// <summary>
  /// The number of times the player has defeated Dreamons with Intimidating Slam.
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

  // Subscribe to the necessary Actions.
  private protected override void OnActivated() => playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;

  // Unsubscribe to any Actions used for the quest.
  private protected override void OnCleanUp() => playerCombat.Weapon.OnWeaponHit -= PlayerWeapon_OnWeaponHit;

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

    // Check if the player Attack State's current Combo Data matches the display name.
    if (playerAttackState.ComboData.DisplayName == DISPLAY_NAME)
    {
      successfulHits++;

      if (victim.CurrentHealth <= damageValue)
        successfulDefeats++;
    }

    // Check if the updated successful hits and defeats meet the goals.
    if (successfulHits >= SuccessfulHitsGoal && successfulDefeats >= SuccessfulDefeatsGoal)
      Complete();
  }
}
