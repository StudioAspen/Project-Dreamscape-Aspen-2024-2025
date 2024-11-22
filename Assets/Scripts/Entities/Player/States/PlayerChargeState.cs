using UnityEngine;

public class PlayerChargeState : PlayerBaseState
{
    public PlayerChargeState(Player player) : base(player)
    {
        this.player = player;
    }

    public override void OnEnter()
    {
        player.PlayAnimation("Charge", 0.25f);

        player.SetSpeedModifier(0);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.PlayAnimation("Charge", 0.25f);

        player.ApplyGravity();

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
        player.GroundedMove();
    }

    public override void FixedUpdate()
    {

    }
}
