using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Charger Memory Ability", menuName = "Memory Abilities/Charger/Target Detected")]
public class PlayerChargerTargetDetectedAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public PlayerChargerChargeAbilityStateSO ChargeState { get; private set; }
    [field: SerializeField] public float TargetDetectedDuration { get; private set; } = 2f;

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
        player.ReplaceOneShotAnimationClip(AnimationClip, "AbilityPlaceholder");
        player.TransitionToAnimation("Ability");

        player.SetSpeedModifier(0f);

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        timer += player.LocalDeltaTime;

        if (timer > TargetDetectedDuration)
        {
            player.PlayerAbilityState.ChangeAbilityState(ChargeState, true);
            return;
        }
    }
}