using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(Player player) : base(player)
    {
        this.player = player;
    }

    public override void OnEnter()
    {
        player.DefaultTransitionToAnimation("JumpingUp");

        player.Jump();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.ApplyGravity();

        if (player.MoveDirection != Vector3.zero)
        {
            player.AccelerateToSpeed(player.MovementSpeed);
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.SetSpeedModifier(0.25f);
            player.AccelerateToSpeed(0f);
        }

        player.RotateToTargetRotation();
        player.InstantlySetGroundedSpeed(player.GetGroundedVelocity().magnitude);
        player.GroundedMove();

        if(player.Velocity.y < 0f)
        {
            player.ChangeState(player.PlayerFallState);
            return;
        }

        if (player.IsGrounded)
        {
            player.ChangeState(player.PlayerIdleState);
            return;
        }
    }

    public override void FixedUpdate()
    {

    }
}
