using UnityEngine;

[System.Serializable]
public class PlayerSlideState : PlayerBaseState
{
    private RaycastHit hitBelow;
    private float hitBelowSlopeAngle;
    public float MovementOnSlopeSpeedModifier { get; private set; } = 1f;

    private Vector3 slideDirection;

    public override void OnEnter()
    {
        player.PlayOneShotAnimation(player.PlayerFallState.AnimationClip);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        if (!CanSlide())
        {
            player.ChangeState(player.DefaultState);
            return;
        }

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

        

        ApplySlide(slideDirection);
    }

    /// <summary>
    /// Calculates the slope angle and checks if the player can slide on the slope.
    /// </summary>
    public void CheckSlopeSliding()
    {
        GetAndSetSlopeSpeedModifierOnAngle(hitBelowSlopeAngle);

        if (!player.IsGrounded)
        {
            hitBelowSlopeAngle = 0f;
            return;
        }

        Physics.Raycast(player.transform.position, Vector3.down, out hitBelow, player.CharacterController.height / 2, player.PhysicsConfig.GroundLayer, QueryTriggerInteraction.Ignore);

        if (hitBelow.collider == null)
        {
            hitBelowSlopeAngle = 0f;
            return;
        }

        Vector3 normal = hitBelow.normal;

        hitBelowSlopeAngle = Vector3.Angle(normal, Vector3.up);

        if (CanSlide())
        {
            slideDirection = Vector3.ProjectOnPlane(Vector3.down, normal);

            player.ChangeState(player.PlayerSlideState);
        }
    }

    /// <summary>
    /// Gets and sets the slope speed modifier based on the ground angle.
    /// </summary>
    /// <param name="groundAngle">The angle of the ground.</param>
    /// <returns>The slope speed modifier.</returns>
    private float GetAndSetSlopeSpeedModifierOnAngle(float groundAngle)
    {
        float slopeSpeedModifier = 1f - (0.15f) * groundAngle / player.CharacterController.slopeLimit;

        if (groundAngle > player.CharacterController.slopeLimit) slopeSpeedModifier = 0.85f;

        MovementOnSlopeSpeedModifier = slopeSpeedModifier;

        return slopeSpeedModifier;
    }

    /// <summary>
    /// Applies the given slide direction to the player's movement.
    /// </summary>
    /// <param name="slideDirection">The direction of the slide.</param>
    private void ApplySlide(Vector3 slideDirection)
    {
        player.SetVelocity(new Vector3(player.Velocity.x, player.PhysicsConfig.GroundedYVelocity, player.Velocity.z));
        player.CharacterController.Move(slideDirection * -player.Velocity.y * player.LocalDeltaTime);
    }

    /// <summary>
    /// Determines whether the player can slide on the current slope.
    /// </summary>
    /// <returns>True if the player can slide, false otherwise.</returns>
    private bool CanSlide()
    {
        if (!player.IsGrounded) return false;
        if (hitBelow.collider == null) return false;
        if (player.CurrentState == player.PlayerAttackState) return false;

        return hitBelowSlopeAngle > player.CharacterController.slopeLimit;
    }
}
