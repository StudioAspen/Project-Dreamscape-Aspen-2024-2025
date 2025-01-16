using System.Collections;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public override void OnEnter()
    {
        player.TransitionToAnimation("Falling", 0.25f);
    }

    public override void OnExit()
    {
       
    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        if (player.IsGrounded)
        {
            player.ChangeState(player.PlayerIdleState);
            return;
        }

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
    }
}