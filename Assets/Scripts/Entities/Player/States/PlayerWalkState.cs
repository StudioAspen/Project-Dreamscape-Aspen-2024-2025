using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public override void OnEnter()
    {
        player.TransitionToAnimation("FlatMovement");

        player.SetSpeedModifier(1f);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
        player.AccelerateToHorizontalSpeed(player.MovementSpeed);
        player.ApplyHorizontalVelocity();

        if (player.MoveDirection == Vector3.zero)
        {
            player.ChangeState(player.PlayerIdleState);
        }

        if (player.PlayerSprintState.IsSprinting)
        {
            player.ChangeState(player.PlayerSprintState);
        }
    }
}
