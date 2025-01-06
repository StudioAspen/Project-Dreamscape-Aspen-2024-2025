using UnityEngine;

public class PlayerSprintingState : PlayerBaseState
{
    private bool isSprintDependentOnTimer = false;
    private float duration;
    private float timer;

    public void SetSprintDuration(float d)
    {
        timer = 0f;
        duration = d;
        isSprintDependentOnTimer = true;
    }

    public override void OnEnter()
    {
        player.TransitionToAnimation("FlatMovement");

        player.SetSpeedModifier(player.SprintSpeedModifier);
    }

    public override void OnExit()
    {
        isSprintDependentOnTimer = false;
        player.IsSprinting = false;
    }

    public override void Update()
    {
        player.ApplyGravity();

        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
        player.AccelerateToHorizontalSpeed(player.MovementSpeed);
        player.ApplyHorizontalVelocity();

        if (player.MoveDirection == Vector3.zero)
        {
            player.ChangeState(player.PlayerIdleState);
            return;
        }

        if (player.IsSprinting) isSprintDependentOnTimer = false;

        if (isSprintDependentOnTimer)
        {
            timer += player.LocalDeltaTime;

            if (timer > duration)
            {
                player.ChangeState(player.PlayerWalkingState);
            }

            return;
        }

        if (!player.IsSprinting)
        {
            player.ChangeState(player.PlayerWalkingState);
        }
    }

    public override void FixedUpdate()
    {
        
        
    }
}
