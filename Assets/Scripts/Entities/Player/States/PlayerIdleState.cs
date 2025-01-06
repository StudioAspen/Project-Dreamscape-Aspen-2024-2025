using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public override void OnEnter()
    {
        player.TransitionToAnimation("FlatMovement");

        player.SetSpeedModifier(0f);
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        player.ApplyGravity();

        player.AccelerateToHorizontalSpeed(0f);
        player.ApplyHorizontalVelocity();

        if (player.MoveDirection != Vector3.zero && player.IsSprinting)
        {
            player.ChangeState(player.PlayerSprintingState);
            return;
        }

        if (player.MoveDirection != Vector3.zero)
        {
            player.ChangeState(player.PlayerWalkingState);
        }
    }

    public override void FixedUpdate()
    {
        
    }
}
