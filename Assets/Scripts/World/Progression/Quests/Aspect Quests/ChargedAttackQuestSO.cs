using UnityEngine;

[CreateAssetMenu(fileName = "_x_Times_y_ByChargedAttackQuestSO", menuName = "World/Progression Quest/Aspect Quests/Charged Attack")]
public class ChargedAttackQuestSO : AspectQuestSO
{
  [field: Header("Config")]
  [field: Range(0, 10)]
  [field: SerializeField] public int SuccessfulHitsGoal { get; private set; }
  [field: Range(0, 10)]
  [field: SerializeField] public int SuccessfulKillsGoal { get; private set; }

  private Player player;
  private PlayerCombat playerCombat; 
  private PlayerAttackState playerAttackState;

  private int successfulHitsByChargedAttack = 0;
  private int successfulKillsByChargedAttack = 0;

  private protected override void OnActivated()
  {
    player = progressionManager?.player;
    playerCombat = player.GetComponent<PlayerCombat>();

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
    if (successfulHitsByChargedAttack >= SuccessfulHitsGoal && successfulKillsByChargedAttack >= SuccessfulKillsGoal)
      Complete();
  }

  private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue) 
  {
    if (player.CurrentState != playerAttackState || playerAttackState.ComboData == null)
      return;

    bool isChargedHit = playerAttackState.ComboData.ComboInputs.Contains(ComboAction.CHARGED_ATTACK1)
            || playerAttackState.ComboData.ComboInputs.Contains(ComboAction.CHARGED_ATTACK2);

    if (isChargedHit)
      successfulHitsByChargedAttack++;

    if (victim.CurrentHealth <= damageValue)
      successfulKillsByChargedAttack++;
  }
}
