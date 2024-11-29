using UnityEngine;

public class PlayerChargeState : PlayerBaseState
{
    public PlayerChargeState(Player player) : base(player)
    {
        this.player = player;
    }

    public override void OnEnter()
    {
        player.TransitionToAnimation("Charge");

        player.SetSpeedModifier(0);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.TransitionToAnimation("Charge");

        if (player.MoveDirection != Vector3.zero)
        {
            player.AccelerateToSpeed(player.MovementSpeed);
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.AccelerateToSpeed(0f);
        }

        player.RotateToTargetRotation();
        player.InstantlySetGroundedSpeed(player.GetGroundedVelocity().magnitude);
    }

    public override void FixedUpdate()
    {
        player.ApplyGravity();
        player.GroundedMove();
    }
}
