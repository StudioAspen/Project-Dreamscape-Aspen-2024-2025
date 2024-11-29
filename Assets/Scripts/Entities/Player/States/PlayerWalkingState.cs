using UnityEngine;

public class PlayerWalkingState : PlayerBaseState
{
    public PlayerWalkingState(Player player) : base(player)
    {
        this.player = player;
    }

    public override void OnEnter()
    {
        player.TransitionToAnimation("FlatMovement");

        player.SetSpeedModifier(1f);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
        player.AccelerateToSpeed(player.MovementSpeed);

        if (player.MoveDirection == Vector3.zero)
        {
            player.ChangeState(player.PlayerIdleState);
        }

        if (player.IsSprinting)
        {
            player.ChangeState(player.PlayerSprintingState);
        }
    }

    public override void FixedUpdate()
    {
        player.ApplyGravity();
        player.GroundedMove();
    }
}
