using System.Collections;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(Player player) : base(player)
    {
        this.player = player;
    }

    public override void OnEnter()
    {
        player.TransitionToAnimation("Falling", 0.25f);
    }

    public override void OnExit()
    {
       
    }

    public override void Update()
    {
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
        
        if (player.IsGrounded)
        {
            player.ChangeState(player.PlayerIdleState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        player.ApplyGravity();
        player.GroundedMove();
    }

}