using UnityEngine;

[CreateAssetMenu(fileName = "Leaper Memory Ground Pound Jump Ability", menuName = "Memory Abilities/Leaper/Ground Pound Jump")]
public class PlayerLeaperGroundPoundJumpAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public AnimationClip JumpAnimationClip { get; private set; }
    [field: SerializeField] public AnimationClip MidAirAnimationClip { get; private set; }
    [field: SerializeField] public PlayerLeaperGroundPoundAbilityStateSO GroundPoundState { get; private set; }
    [field: SerializeField] public float JumpHeight { get; private set; } = 5f;
    [field: SerializeField] public float MidAirPauseDuration { get; private set; } = 1f;

    private bool hasMidAirPauseStarted;
    private float midAirTimer;

    public override bool CanUseAbility(Player player)
    {
        return GroundPoundState.CanUseAbility(player);
    }

    public override bool CanCancelAbility(Player player, EntityBaseState desiredState)
    {
        return desiredState == player.PlayerAbilityState || desiredState == player.DefaultState;
    }

    public override void OnEnter()
    {
        player.PlayOneShotAnimation(JumpAnimationClip);

        player.PlayerJumpState.Jump(JumpHeight);

        player.SetSpeedModifier(0f);

        hasMidAirPauseStarted = false;
        midAirTimer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        if(!hasMidAirPauseStarted) player.ApplyGravity();

        if(midAirTimer >= MidAirPauseDuration)
        {
            player.PlayerAbilityState.TryChangeAbilityState(GroundPoundState, true);
            return;
        }

        if (hasMidAirPauseStarted) midAirTimer += player.LocalDeltaTime;

        if (player.Velocity.y < 0f && !hasMidAirPauseStarted)
        {
            hasMidAirPauseStarted = true;
            player.PlayOneShotAnimation(MidAirAnimationClip, MidAirPauseDuration);
        }

        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
    }
}