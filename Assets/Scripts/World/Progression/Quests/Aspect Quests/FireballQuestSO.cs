using UnityEngine;

[CreateAssetMenu(fileName = "_x_TimesFireballAspectQuestSO", menuName = "World/Progression Quest/Aspect Quests/Fireball")]

public class FireballQuestSO : AspectQuestSO
{
  [field: Header("Config")]
  [field: Range(1, 10)]
  [field: SerializeField] public int SuccessfulHitsGoal { get; private set; }
  [field: SerializeField] public ComboDataSO TargetComboData { get; private set; } 

  private Player player;
  private PlayerCombat playerCombat; 
  private PlayerAttackState playerAttackState;
  private int successfulHits = 0;

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
    if (IsCompleted)
      return;

    if (successfulHits >= SuccessfulHitsGoal)
      Complete();
  }

  private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue) 
  {
    if (player.CurrentState != playerAttackState || playerAttackState.ComboData == null)
      return;

    if (playerAttackState.ComboData.DisplayName == TargetComboData.DisplayName)
      successfulHits++;
  }
}