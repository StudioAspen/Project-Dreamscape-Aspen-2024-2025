using UnityEngine;

public class PlayerSprintState : PlayerBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float SprintSpeedModifier { get; private set; } = 1.66f;

    [HideInInspector] public bool IsSprinting;

    private bool isSprintDependentOnTimer = false;
    private float duration;
    private float timer;

    /// <summary>
    /// Sets the duration of the sprint, after which the player will stop sprinting.
    /// </summary>
    /// <param name="sprintDuration">The duration of the sprint.</param>
    public void SetSprintDuration(float sprintDuration)
    {
        timer = 0f;
        duration = sprintDuration;
        isSprintDependentOnTimer = true;
    }

    public override void OnEnter()
    {
        player.PlayDefaultAnimation();

        player.SetSpeedModifier(SprintSpeedModifier);
    }

    public override void OnExit()
    {
        isSprintDependentOnTimer = false;
    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        if (player.MoveDirection == Vector3.zero)
        {
            player.ChangeState(player.PlayerIdleState);
            return;
        }

        if (player.PlayerSprintState.IsSprinting) isSprintDependentOnTimer = false;

        if (isSprintDependentOnTimer)
        {
            timer += player.LocalDeltaTime;

            if (timer > duration)
            {
                player.ChangeState(player.PlayerWalkState);
                return;
            }
        }

        if (!player.PlayerSprintState.IsSprinting && !isSprintDependentOnTimer)
        {
            player.ChangeState(player.PlayerWalkState);
            return;
        }

        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
        player.AccelerateToHorizontalSpeed(player.MovementSpeed);
        player.ApplyHorizontalVelocity();
    }

    /// <summary>
    /// Determines whether the player can sprint.
    /// </summary>
    /// <returns>True if the player can sprint, false otherwise.</returns>
    public bool CanSprint()
    {
        if (player.CurrentState == player.PlayerChargeState) return false;
        if (player.CurrentState == player.PlayerDashState) return false;

        return true;
    }
}
