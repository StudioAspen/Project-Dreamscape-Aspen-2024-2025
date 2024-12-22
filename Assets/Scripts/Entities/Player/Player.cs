using KBCore.Refs;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Player : Entity
{
    [Header("Player: References")]
    [SerializeField, Self] private PlayerInputReader playerInputReader;

    [field: Header("Player: Grounded Movement")]
    [field: SerializeField] public float SprintSpeedModifier { get; private set; } = 1.66f;
    [SerializeField] private float groundedAcceleration = 4f;
    public Vector3 MoveDirection => playerInputReader.MoveDirection;
    private float movementOnSlopeSpeedModifier = 1f;
    private float forwardAngleBasedOnCamera;
    private Quaternion targetForwardRotation = Quaternion.identity;
    private Vector3 targetForwardDirection = Vector3.forward;
    private RaycastHit hitBelow;
    private float hitBelowSlopeAngle;

    [Header("Player: Gravity")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private int maxJumpCount = 1;
    private int currentJumpCount;
    
    #region Flags
    [HideInInspector] public bool IsMoving => playerInputReader.MoveDirection.sqrMagnitude > 0;
    [HideInInspector] public bool IsSprinting;
    private bool isJumping;
    #endregion

    [field : Header("Player: Dash")]
    [field: SerializeField] public float DashDuration { get; private set; } = 0.5f;
    [field : SerializeField] public float InitialDashVelocity { get; private set; } = 25f;
    [field: SerializeField] public float SprintDurationAfterDash { get; private set; } = 2f;
    [SerializeField] private float dashDelayDuration = 1f;
    [SerializeField] private GameObject dashTrailObject;
    private float dashDelayTimer = Mathf.Infinity;
    private Coroutine dashCoroutine;

    #region States 
    public PlayerIdleState PlayerIdleState { get; private set; }
    public PlayerWalkingState PlayerWalkingState { get; private set; }
    public PlayerSprintingState PlayerSprintingState { get; private set; }
    public PlayerJumpState PlayerJumpState { get; private set; }
    public PlayerFallState PlayerFallState { get; private set; }
    public PlayerDashState PlayerDashState { get; private set; }
    public PlayerSlideState PlayerSlideState { get; private set; }
    public PlayerAttackState PlayerAttackState { get; private set; }
    public PlayerChargeState PlayerChargeState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        PlayerIdleState = new PlayerIdleState(this);
        PlayerWalkingState = new PlayerWalkingState(this);
        PlayerSprintingState = new PlayerSprintingState(this);
        PlayerJumpState = new PlayerJumpState(this);
        PlayerFallState = new PlayerFallState(this);
        PlayerDashState = new PlayerDashState(this);
        PlayerSlideState = new PlayerSlideState(this);
        PlayerAttackState = new PlayerAttackState(this);
        PlayerChargeState = new PlayerChargeState(this);
    }
    #endregion

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnStart()
    {
        base.OnStart();

        ChangeTeam(0);

        SetStartState(PlayerIdleState);
        SetDefaultState(PlayerIdleState);
    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();

        CheckSlopeSliding();

        HandleDashDelay();
        HandleDashTrail();
    }

    /// <summary>
    /// Determines whether the player can perform a jump.
    /// </summary>
    /// <returns>True if the player can jump, false otherwise.</returns>
    public bool CanJump()
    {
        if (!IsGrounded && (currentJumpCount >= maxJumpCount || maxJumpCount == 1)) return false;
        if (CurrentState == EntityLaunchState) return false;
        if (CurrentState == PlayerSlideState) return false;
        if (CurrentState == PlayerChargeState) return false;
        if (CurrentState == PlayerAttackState) return false;
        if (CurrentState == EntityStaggeredState) return false;

        return true;
    }

    /// <summary>
    /// Determines whether the player can perform a dash.
    /// </summary>
    /// <returns>True if the player can dash, false otherwise.</returns>
    public bool CanDash()
    {
        if (dashDelayTimer < dashDelayDuration) return false;
        if (CurrentState == PlayerChargeState) return false;
        if (CurrentState == PlayerDashState) return false;
        if (CurrentState == EntityStaggeredState) return false;
        if (CurrentState == EntityLaunchState) return false;

        return true;
    }

    /// <summary>
    /// Determines whether the player can sprint.
    /// </summary>
    /// <returns>True if the player can sprint, false otherwise.</returns>
    public bool CanSprint()
    {
        if (CurrentState == PlayerChargeState) return false;
        if (CurrentState == PlayerDashState) return false;

        return true;
    }

    /// <summary>
    /// Accelerates the player's horizontal velocity to the specified speed.
    /// </summary>
    /// <param name="speed">The target speed to accelerate to.</param>
    public void AccelerateToHorizontalSpeed(float speed)
    {
        Vector3 horizontalVelocity = GetHorizontalVelocity();

        horizontalVelocity = Vector3.Lerp(horizontalVelocity, speed * targetForwardDirection, groundedAcceleration * LocalDeltaTime);

        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
    }

    /// <summary>
    /// Instantly sets the horizontal speed of the player to the specified value.
    /// </summary>
    /// <param name="speed">The target speed to set.</param>
    public void InstantlySetHorizontalSpeed(float speed)
    {
        Vector3 horizontalVelocity = GetHorizontalVelocity();

        horizontalVelocity = speed * targetForwardDirection;

        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
    }

    private protected override void HandleGrounded()
    {
        if (IsGrounded)
        {
            if (CurrentState != PlayerSlideState)
            {
                currentJumpCount = 0;
            }
            inAirTimer = 0f;
            fallVelocityApplied = false;
            isJumping = false;
            velocity.y = PhysicsSettings.GroundedYVelocity;
        }
    }

    private protected override void HandleAirborne()
    {
        if (!IsGrounded)
        {
            if (!isJumping && !fallVelocityApplied) // falling without jumping
            {
                fallVelocityApplied = true;
                velocity.y = PhysicsSettings.FallingStartingYVelocity;
            }
            if (CanBeForcedToFall())
            {
                ChangeState(PlayerFallState);
            }
            inAirTimer += LocalDeltaTime;
        }
    }

    /// <summary>
    /// Determines whether the player can be forced to fall based on the current state.
    /// </summary>
    /// <returns>True if the player can be forced to fall, false otherwise.</returns>
    private bool CanBeForcedToFall()
    {
        bool willNotFall = CurrentState == PlayerJumpState
            || CurrentState == PlayerFallState
            || CurrentState == PlayerDashState
            || CurrentState == PlayerChargeState
            || CurrentState == PlayerAttackState
            || CurrentState == EntityLaunchState;

        return !willNotFall;
    }

    /// <summary>
    /// Rotates the player to the target rotation over time.
    /// Must be called in update to work.
    /// </summary>
    public void RotateToTargetRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetForwardRotation, rotationSpeed * LocalDeltaTime);
    }

    /// <summary>
    /// Handles the delay for the dash ability.
    /// Counts up the timer.
    /// </summary>
    private void HandleDashDelay()
    {
        dashDelayTimer += LocalDeltaTime;
    }

    /// <summary>
    /// Resets the dash delay timer.
    /// </summary>
    public void ResetDashDelay()
    {
        dashDelayTimer = 0f;
    }

    /// <summary>
    /// Calculates the slope angle and checks if the player can slide on the slope.
    /// </summary>
    private void CheckSlopeSliding()
    {
        GetAndSetSlopeSpeedModifierOnAngle(hitBelowSlopeAngle);

        if (!IsGrounded)
        {
            hitBelowSlopeAngle = 0f;
            return;
        }

        Physics.Raycast(transform.position, Vector3.down, out hitBelow, controller.height / 2, PhysicsSettings.GroundLayer, QueryTriggerInteraction.Ignore);

        if (hitBelow.collider == null)
        {
            hitBelowSlopeAngle = 0f;
            return;
        }

        Vector3 normal = hitBelow.normal;

        hitBelowSlopeAngle = Vector3.Angle(normal, Vector3.up);

        if (CanSlide())
        {
            Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, normal);

            PlayerSlideState.SetSlideDirection(slideDirection);
            ChangeState(PlayerSlideState);
        }
    }

    /// <summary>
    /// Determines whether the player can slide on the current slope.
    /// </summary>
    /// <returns>True if the player can slide, false otherwise.</returns>
    public bool CanSlide()
    {
        if (!IsGrounded) return false;
        if (hitBelow.collider == null) return false;
        if (CurrentState == PlayerAttackState) return false;

        return hitBelowSlopeAngle > controller.slopeLimit;
    }

    /// <summary>
    /// Applies the given slide direction to the player's movement.
    /// </summary>
    /// <param name="slideDirection">The direction of the slide.</param>
    public void ApplySlide(Vector3 slideDirection)
    {
        velocity.y = PhysicsSettings.GroundedYVelocity;
        controller.Move(slideDirection * -velocity.y * LocalDeltaTime);
    }

    /// <summary>
    /// Applies the calculated rotation based off the player's movement and camera to the next movement.
    /// Doesn't actually rotate the player until you call RotateToTargetRotation().
    /// </summary>
    public void ApplyRotationToNextMovement()
    {
        forwardAngleBasedOnCamera = Mathf.Atan2(playerInputReader.MoveDirection.x, playerInputReader.MoveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
        targetForwardRotation = Quaternion.Euler(0, forwardAngleBasedOnCamera, 0);
        targetForwardDirection = targetForwardRotation * Vector3.forward;
    }

    /// <summary>
    /// Applies the given target rotation to the next movement.
    /// Doesn't actually rotate the player until you call RotateToTargetRotation().
    /// </summary>
    /// <param name="targetRotation">The target rotation to apply.</param>
    public void ApplyRotationToNextMovement(Quaternion targetRotation)
    {
        targetForwardRotation = targetRotation;
        targetForwardDirection = targetForwardRotation * Vector3.forward;
    }

    private protected override void EvaluateMovementSpeed()
    {
        MovementSpeed = movementOnSlopeSpeedModifier * StatusSpeedModifier * SpeedModifier * baseSpeed;
    }

    /// <summary>
    /// Makes the player jump by setting the necessary variables and applying the jump force.
    /// </summary>
    public void Jump()
    {
        IsGrounded = false;

        isJumping = true;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * PhysicsSettings.Gravity);
        inAirTimer = 0.01f;

        currentJumpCount++;
    }

    /// <summary>
    /// Gets and sets the slope speed modifier based on the ground angle.
    /// </summary>
    /// <param name="groundAngle">The angle of the ground.</param>
    /// <returns>The slope speed modifier.</returns>
    private float GetAndSetSlopeSpeedModifierOnAngle(float groundAngle)
    {
        float slopeSpeedModifier = 1f - (0.15f) * groundAngle / controller.slopeLimit;

        if (groundAngle > controller.slopeLimit) slopeSpeedModifier = 0.85f;

        movementOnSlopeSpeedModifier = slopeSpeedModifier;

        return slopeSpeedModifier;
    }

    /// <summary>
    /// Gets the maximum speed of the player, taking into account the sprint speed modifier.
    /// </summary>
    /// <returns>The maximum speed of the player.</returns>
    public float GetMaxSpeed()
    {
        return SprintSpeedModifier * baseSpeed;
    }

    /// <summary>
    /// Handles the dash trail effect based on the player's speed.
    /// </summary>
    private void HandleDashTrail()
    {
        float maxSpeed = SprintSpeedModifier * baseSpeed;

        dashTrailObject.SetActive(GetHorizontalVelocity().magnitude > maxSpeed);
    }

    private protected override void TryChangeStaggeredState()
    {
        if (CurrentState == PlayerDashState) return;
        if (CurrentState == PlayerChargeState) return;
        if (CurrentState == PlayerAttackState) return;
        if (CurrentState == EntityLaunchState) return;

        ForceChangeState(EntityStaggeredState);
    }

    /// <summary>
    /// Simulates launching the player by performing a fake jump and launching them in the specified direction with the given force.
    /// </summary>
    /// <param name="direction">The direction in which the player should be launched.</param>
    /// <param name="force">The force with which the player should be launched.</param>
    public override void Launch(Vector3 direction, float force)
    {
        // Calculate the resulting change in velocity from the impulse
        Vector3 deltaVelocity = (force * direction.normalized) / mass;

        IsGrounded = false;
        isJumping = true;
        inAirTimer = 0.01f;
        currentJumpCount++;

        // Apply the change to the current velocity
        velocity = deltaVelocity;
    }
}
