using UnityEngine;

[CreateAssetMenu(fileName = "_x_TimesFlameSwingQuestSO", menuName = "World/Progression Quest/Aspect Quests/Flame Swing")]
public class FlameSwingQuestSO : AspectQuestSO
{
  [field: Header("Flame Swing Configuration")]

  /// <summary>
  /// The number of times the player must hit Dreamons with Flame Swing to complete the quest.
  /// </summary>
  [field: Tooltip("The number of times the player must hit Dreamons with Flame Swing to complete the quest.")]
  [field: Range(1, 10)]
  [field: SerializeField] public int SuccessfulHitsGoal { get; private set; }

  /// <summary>
  /// The display name of the Flame Swing Combo Data.
  /// </summary>
  private const string DISPLAY_NAME = "Flame Swing";

  /// <summary>
  /// The number of times the player has hit Dreamons with Flame Swing.
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

  private protected override void OnActivated() => playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;

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

    if (playerAttackState.ComboData.DisplayName == DISPLAY_NAME)
      successfulHits++;

    // Check if the updated successful hits meets the goal.
    if (successfulHits >= SuccessfulHitsGoal)
      Complete();
  }
}