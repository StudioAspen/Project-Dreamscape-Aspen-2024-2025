using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Charger Memory Charge Ability", menuName = "Memory Abilities/Charger/Charge")]
public class PlayerChargerChargeAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public AnimationClip ChargeAnimationClip { get; private set; }
    [field: SerializeField] public PlayerChargerWindDownAbilityStateSO WindDownState { get; private set; }
    [field: SerializeField] public float ChargeContactDamageMultiplier { get; private set; } = 2f;
    [field: SerializeField] public float ChargeSpeedModifier { get; private set; } = 3f;
    [field: SerializeField] public float ChargeDuration { get; private set; } = 20f;
    [field: SerializeField] public float ChargeRotationSpeed { get; private set; } = 5f;
    [field: SerializeField] public float ChargeOnImpactLaunchForce { get; private set; } = 10f;
    [field: SerializeField] public float ChargeStunDuration { get; private set; } = 4f;

    private float timer;

    public override bool CanUseAbility(Player player)
    {
        bool cannotUseAbility =
            !player.IsGrounded ||
            player.CurrentState == player.PlayerAttackState ||
            player.CurrentState == player.PlayerChargeState ||
            player.CurrentState == player.PlayerDashState;

        return !cannotUseAbility;
    }

    public override bool CanCancelAbility(Player player, EntityBaseState desiredState)
    {
        return desiredState == player.PlayerAbilityState || desiredState == player.DefaultState;
    }

    public override void OnEnter()
    {
        player.PlayOneShotAnimation(ChargeAnimationClip);

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
            player.PlayerAbilityState.TryChangeAbilityState(WindDownState, true);
            return;
        }

        player.ApplyRotationToNextMovement();
        player.LookAt(player.transform.position + player.TargetForwardDirection, ChargeRotationSpeed);

        player.UpdateHorizontalVelocity(player.transform.forward);
        player.ApplyHorizontalVelocity();
    }

    public override void OnOnControllerColliderHit(ControllerColliderHit hit)
    {
        if (player.DidHitEnemyEntity(hit.collider, out Entity enemyEntity))
        {
            CameraShakeManager.Instance.ShakeCamera(2f, 1f, 0.25f);

            Vector3 launchDirection = enemyEntity.GetColliderCenterPosition() - player.transform.position;
            enemyEntity.TryChangeToLaunchState(player, launchDirection, ChargeOnImpactLaunchForce, ChargeStunDuration);

            player.DealDamageToOtherEntity(enemyEntity, player.CalculateDamage(ChargeContactDamageMultiplier), hit.point, false);

            //player.PlayerAbilityState.TryChangeAbilityState(WindDownState, true);
            return;
        }
    }
}
