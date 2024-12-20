using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerDashState : PlayerBaseState
{
    private float timer;

    private float currDashSpeed;
    private float maxSpeed;

    public PlayerDashState(Player player) : base(player)
    {
        this.player = player;
    }

    public override void OnEnter()
    {
        player.ResetDashDelay();

        player.TransitionToAnimation("Dash");

        timer = 0f;
        currDashSpeed = player.InitialDashVelocity;
        maxSpeed = player.GetMaxSpeed();

        player.ApplyRotationToNextMovement();

        player.InstantlySetHorizontalSpeed(player.InitialDashVelocity);
    }

    public override void OnExit()
    {
        player.InstantlySetHorizontalSpeed(maxSpeed);
        player.ResetYVelocity();
    }

    public override void Update()
    {
        timer += player.LocalDeltaTime;

        currDashSpeed = (player.InitialDashVelocity - maxSpeed) * (1 - Mathf.Sqrt(1 - Mathf.Pow(timer / player.DashDuration - 1, 2))) + maxSpeed;

        if (player.MoveDirection != Vector3.zero) player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();

        player.InstantlySetHorizontalSpeed(currDashSpeed);
        player.ApplyHorizontalVelocity();

        if (timer > player.DashDuration)
        {
            if (!player.IsGrounded)
            {
                player.ChangeState(player.PlayerFallState);
            }
            else
            {
                player.PlayerSprintingState.SetSprintDuration(player.SprintDurationAfterDash);
                player.ChangeState(player.PlayerSprintingState);
            }  
        }

        player.ResetDashDelay(); // keeps dash delay timer at 0 so that once you stop dashing, the timer goes up
    }

    public override void FixedUpdate()
    {
        
    }
}
