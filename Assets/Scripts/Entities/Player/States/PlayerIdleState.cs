using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(Player player) : base(player)
    {
        this.player = player;
    }

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
        player.AccelerateToSpeed(0f);
        
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
        player.ApplyGravity();
        player.GroundedMove();
    }
}
