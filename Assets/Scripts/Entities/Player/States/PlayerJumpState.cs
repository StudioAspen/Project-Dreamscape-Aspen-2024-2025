using UnityEngine;

[System.Serializable]
public class PlayerJumpState : PlayerBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float JumpHeight { get; private set; } = 2f;
    [field: SerializeField] public int MaxJumpCount { get; private set; } = 1;

    [HideInInspector] public bool IsJumping;
    private bool canJumpMidair => currentJumpCount < MaxJumpCount && MaxJumpCount > 1;
    private int currentJumpCount;

    public override void OnEnter()
    {
        player.PlayOneShotAnimation(AnimationClip);
        AkSoundEngine.PostEvent("PlayerJump", player.gameObject);

        Jump();

        currentJumpCount++;
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        if (player.Velocity.y < 0f)
        {
            player.ChangeState(player.PlayerFallState);
            return;
        }

        if (player.MoveDirection != Vector3.zero)
        {
            player.AccelerateToHorizontalSpeed(player.MovementSpeed);
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.SetSpeedModifier(0.25f);
            player.AccelerateToHorizontalSpeed(0f);
        }

        player.RotateToTargetRotation();
        player.InstantlySetHorizontalSpeed(player.GetHorizontalVelocity().magnitude);
        player.ApplyHorizontalVelocity();
    }

    /// <summary>
    /// Makes the player jump by setting the necessary variables and applying the jump force.
    /// </summary>
    public void Jump()
    {
        player.AllowChangeFromGroundedToAirborne();

        player.IsGrounded = false;

        IsJumping = true;

        player.SetVelocity(new Vector3(player.Velocity.x, Mathf.Sqrt(JumpHeight * -2f * player.PhysicsConfig.Gravity), player.Velocity.z));
    }

    /// <summary>
    /// Makes the player jump by setting the necessary variables and applying the jump force.
    /// </summary>
    /// <param name="customJumpHeight">The custom jump height to apply.</param>
    public void Jump(float customJumpHeight)
    {
        player.AllowChangeFromGroundedToAirborne();

        player.IsGrounded = false;

        IsJumping = true;

        player.SetVelocity(new Vector3(player.Velocity.x, Mathf.Sqrt(customJumpHeight * -2f * player.PhysicsConfig.Gravity), player.Velocity.z));
    }

    /// <summary>
    /// Resets the current jump count to zero.
    /// </summary>
    public void ResetJumpCount()
    {
        currentJumpCount = 0;
    }

    /// <summary>
    /// Determines whether the player can perform a jump.
    /// </summary>
    /// <returns>True if the player can jump, false otherwise.</returns>
    public bool CanJump()
    {
        if (!player.IsGrounded && !canJumpMidair) return false;
        if (player.CurrentState == player.EntityLaunchState) return false;
        if (player.CurrentState == player.PlayerSlideState) return false;
        if (player.CurrentState == player.PlayerChargeState) return false;
        if (player.CurrentState == player.PlayerAttackState) return false;
        if (player.CurrentState == player.EntityStaggeredState) return false;
        if (player.CurrentState == player.EntityStunnedState) return false;

        return true;
    }
}
