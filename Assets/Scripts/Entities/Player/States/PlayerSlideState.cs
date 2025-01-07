using UnityEngine;

public class PlayerSlideState : PlayerBaseState
{
    private Vector3 slideDirection;

    public PlayerSlideState(Player player) : base(player)
    {
        this.player = player;
    }

    public void SetSlideDirection(Vector3 dir)
    {
        slideDirection = dir;
    }

    public override void OnEnter()
    {
        player.TransitionToAnimation("Falling");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.ApplyGravity();

        player.IsGrounded = true;

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

        if (!player.CanSlide()) player.ChangeState(player.DefaultState);

        player.ApplySlide(slideDirection);
    }

    public override void FixedUpdate()
    {

    }
}
