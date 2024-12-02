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
        player.ApplyGravity();

        player.TransitionToAnimation("Charge");

        if (player.MoveDirection != Vector3.zero)
        {
            player.AccelerateToHorizontalSpeed(player.MovementSpeed);
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.AccelerateToHorizontalSpeed(0f);
        }

        player.RotateToTargetRotation();
        player.InstantlySetHorizontalSpeed(player.GetHorizontalVelocity().magnitude);
        player.ApplyHorizontalVelocity();
    }

    public override void FixedUpdate()
    {

    }
}
