using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Player : Entity
{
    private PlayerInputReader playerInputReader;

    [Header("Player: References")]
    [SerializeField] private GameObject dashTrailObject;

    [field: Header("Player: Grounded Movement")]
    [field: SerializeField] public float SprintSpeedModifier { get; private set; } = 1.66f;
    [SerializeField] private float groundedAcceleration = 4f;
    public Vector3 MoveDirection => playerInputReader.MoveDirection;
    /// <summary>
    /// Gets the maximum speed of the player, taking into account the sprint speed modifier.
    /// </summary>
    /// <returns>The maximum speed of the player.</returns>

    public float MaxSpeed => SprintSpeedModifier * baseSpeed;
    private float forwardAngleBasedOnCamera;
    private Quaternion targetForwardRotation = Quaternion.identity;
    private Vector3 targetForwardDirection = Vector3.forward;
    [HideInInspector] public bool IsMoving => playerInputReader.MoveDirection.sqrMagnitude > 0;

    #region States 
    [field: Header("Player: States")]
    [field: SerializeField] public PlayerDashStateSO PlayerDashState { get; private set; }
    [field: SerializeField] public PlayerJumpStateSO PlayerJumpState { get; private set; }
    public PlayerIdleStateSO PlayerIdleState { get; private set; }
    public PlayerWalkStateSO PlayerWalkState { get; private set; }
    public PlayerSprintStateSO PlayerSprintState { get; private set; }
    public PlayerFallStateSO PlayerFallState { get; private set; }
    public PlayerSlideStateSO PlayerSlideState { get; private set; }
    public PlayerAttackStateSO PlayerAttackState { get; private set; }
    public PlayerChargeStateSO PlayerChargeState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        PlayerDashState = EntityBaseStateSO.CreateRuntimeInstance<PlayerDashStateSO>(PlayerDashState, this);
        PlayerJumpState = EntityBaseStateSO.CreateRuntimeInstance<PlayerJumpStateSO>(PlayerJumpState, this);

        PlayerIdleState = EntityBaseStateSO.CreateRuntimeInstance<PlayerIdleStateSO>(this);
        PlayerWalkState = EntityBaseStateSO.CreateRuntimeInstance<PlayerWalkStateSO>(this);
        PlayerSprintState = EntityBaseStateSO.CreateRuntimeInstance<PlayerSprintStateSO>(this);
        PlayerFallState = EntityBaseStateSO.CreateRuntimeInstance<PlayerFallStateSO>(this);
        PlayerSlideState = EntityBaseStateSO.CreateRuntimeInstance<PlayerSlideStateSO>(this);
        PlayerAttackState = EntityBaseStateSO.CreateRuntimeInstance<PlayerAttackStateSO>(this);
        PlayerChargeState = EntityBaseStateSO.CreateRuntimeInstance<PlayerChargeStateSO>(this);
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

        playerInputReader = GetComponent<PlayerInputReader>();
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

        PlayerSlideState.CheckSlopeSliding();

        PlayerDashState.HandleDashCooldown();
        HandleDashTrail();
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
                PlayerJumpState.ResetJumpCount();
            }
            inAirTimer = 0f;
            fallVelocityApplied = false;
            PlayerJumpState.IsJumping = false;
            velocity.y = PhysicsSettings.GroundedYVelocity;
        }
    }

    private protected override void HandleAirborne()
    {
        if (!IsGrounded)
        {
            if (!PlayerJumpState.IsJumping && !fallVelocityApplied) // falling without jumping
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
        MovementSpeed = PlayerSlideState.MovementOnSlopeSpeedModifier * StatusSpeedModifier * SpeedModifier * baseSpeed;
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

        ChangeState(EntityStaggeredState, true);
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

        AllowChangeFromGroundedToAirborne();

        IsGrounded = false;
        PlayerJumpState.IsJumping = true;

        // Apply the change to the current velocity
        velocity = deltaVelocity;
    }
}
