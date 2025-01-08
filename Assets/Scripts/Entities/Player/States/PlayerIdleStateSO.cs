using UnityEngine;

public class PlayerIdleStateSO : PlayerBaseStateSO
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

        if (player.MoveDirection != Vector3.zero && player.PlayerSprintState.IsSprinting)
        {
            player.ChangeState(player.PlayerSprintState);
            return;
        }

        if (player.MoveDirection != Vector3.zero)
        {
            player.ChangeState(player.PlayerWalkState);
        }
    }

    public override void FixedUpdate()
    {
        
    }
}
