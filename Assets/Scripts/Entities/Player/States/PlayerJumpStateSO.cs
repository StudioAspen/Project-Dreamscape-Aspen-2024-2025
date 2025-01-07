using UnityEngine;

public class PlayerJumpStateSO : PlayerBaseStateSO
{
    public override void OnEnter()
    {
        player.TransitionToAnimation("JumpingUp");

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
            player.AccelerateToHorizontalSpeed(player.MovementSpeed);
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.SetSpeedModifier(0.25f);
            player.AccelerateToHorizontalSpeed(0f);
        }

        player.RotateToTargetRotation();
        player.InstantlySetHorizontalSpeed(player.GetHorizontalVelocity().magnitude);
        player.ApplyHorizontalVelocity();

        if (player.Velocity.y < 0f)
        {
            player.ChangeState(player.PlayerFallState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        
    }
}
