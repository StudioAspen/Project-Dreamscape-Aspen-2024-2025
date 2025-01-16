using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Charger Memory Ability", menuName = "Memory Abilities/Charger")]
public class PlayerChargerAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public float ChargeContactPercentDamage { get; private set; } = 200f;
    [field: SerializeField] public float ChargeSpeedModifier { get; private set; } = 3f;
    [field: SerializeField] public float ChargeDuration { get; private set; } = 20f;
    [field: SerializeField] public float ChargeRotationSpeed { get; private set; } = 5f;
    [field: SerializeField] public float ChargeOnImpactLaunchForce { get; private set; } = 10f;
    [field: SerializeField] public float ChargeStunDuration { get; private set; } = 4f;

    private float timer;

    public override bool CanUseAbility()
    {
        bool cannotUseAbility =
            !player.IsGrounded ||
            player.CurrentState == player.PlayerAttackState ||
            player.CurrentState == player.PlayerChargeState ||
            player.CurrentState == player.PlayerDashState;

        return !cannotUseAbility;
    }

    public override bool CanCancelAbility(EntityBaseState desiredState)
    {
        bool cannotCancelAbility =
            desiredState == player.PlayerAttackState ||
            desiredState == player.PlayerChargeState ||
            desiredState == player.PlayerDashState ||
            desiredState == player.PlayerJumpState ||
            desiredState == player.PlayerFallState ||
            desiredState == player.PlayerSlideState ||
            desiredState == player.EntityStaggeredState ||
            desiredState == player.EntityLaunchState;

        return !cannotCancelAbility;
    }

    public override void OnEnter()
    {
        player.TransitionToAnimation("Dash");

        player.SetSpeedModifier(ChargeSpeedModifier);

        timer = 0f;
    }

    public override void OnExit()
    {
       
    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        timer += player.LocalDeltaTime;
        if (timer > ChargeDuration)
        {
            player.ChangeState(player.DefaultState);
            return;
        }

        player.UpdateHorizontalVelocity(player.transform.forward);
        player.ApplyHorizontalVelocity();

        player.ApplyRotationToNextMovement();
        player.LookAt(player.transform.position + player.TargetForwardDirection, ChargeRotationSpeed);
    }

    public override void OnOnControllerColliderHit(ControllerColliderHit hit)
    {
        if (player.DidHitEnemyEntity(hit.collider, out Entity enemyEntity))
        {
            CameraShakeManager.Instance.ShakeCamera(2f, 0.25f);

            Vector3 launchDirection = enemyEntity.GetColliderCenterPosition() - player.transform.position;
            enemyEntity.TryChangeToLaunchState(launchDirection, ChargeOnImpactLaunchForce, ChargeStunDuration);

            enemyEntity.TakeDamage(player.CalculateDamage(ChargeContactPercentDamage), hit.point, player.gameObject, false);

            player.ChangeState(player.DefaultState);
            return;
        }
    }
}