using System;
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

    /// <summary>
    /// Action that is invoked when the player is first instantiated.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Player player</c>: The instantiated player</description></item>
    /// </list>
    /// </remarks>
    public static Action<Player> OnPlayerInstantiated = delegate { };
    /// <summary>
    /// Action that is invoked when the player is destroyed.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Player player</c>: The destroyed player</description></item>
    /// </list>
    /// </remarks>
    public static Action<Player> OnPlayerDestroyed = delegate { };

    [field: Header("Player: Grounded Movement")]
    [SerializeField] private float groundedAcceleration = 4f;
    public Vector3 MoveDirection => playerInputReader.MoveDirection;
    /// <summary>
    /// Gets the maximum speed of the player, taking into account the sprint speed modifier.
    /// </summary>
    /// <returns>The maximum speed of the player.</returns>
    public float MaxSpeed => PlayerSprintState.SprintSpeedModifier * baseSpeed;
    private float forwardAngleBasedOnCamera;
    public Quaternion TargetForwardRotation { get; private set; } = Quaternion.identity;
    public Vector3 TargetForwardDirection { get; private set; } = Vector3.forward;
    [HideInInspector] public bool IsMoving => playerInputReader.MoveDirection != Vector3.zero;

    #region States 
    [field: Header("Player: States")]
    public PlayerIdleState PlayerIdleState { get; private set; }
    public PlayerWalkState PlayerWalkState { get; private set; }
    public PlayerSprintState PlayerSprintState { get; private set; }
    public PlayerDashState PlayerDashState { get; private set; }
    public PlayerJumpState PlayerJumpState { get; private set; }
    public PlayerFallState PlayerFallState { get; private set; }
    public PlayerSlideState PlayerSlideState { get; private set; }
    public PlayerAttackState PlayerAttackState { get; private set; }
    public PlayerChargeState PlayerChargeState { get; private set; }
    public PlayerAbilityState PlayerAbilityState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        PlayerIdleState = EntityBaseState.InitializeOrCreate<PlayerIdleState>(this);
        PlayerWalkState = EntityBaseState.InitializeOrCreate<PlayerWalkState>(this);
        PlayerSprintState = EntityBaseState.InitializeOrCreate<PlayerSprintState>(this);
        PlayerDashState = EntityBaseState.InitializeOrCreate<PlayerDashState>(this);
        PlayerJumpState = EntityBaseState.InitializeOrCreate<PlayerJumpState>(this);
        PlayerFallState = EntityBaseState.InitializeOrCreate<PlayerFallState>(this);
        PlayerSlideState = EntityBaseState.InitializeOrCreate<PlayerSlideState>(this);
        PlayerAttackState = EntityBaseState.InitializeOrCreate<PlayerAttackState>(this);
        PlayerChargeState = EntityBaseState.InitializeOrCreate<PlayerChargeState>(this);
        PlayerAbilityState = EntityBaseState.InitializeOrCreate<PlayerAbilityState>(this);
    }

    public override void ChangeState(EntityBaseState state, bool willForceChange = false)
    {
        if (CurrentState == EntityDeathState) return;
        if (!willForceChange && CurrentState == state) return;
        if (CurrentState == PlayerAbilityState && !PlayerAbilityState.CanCancelAbility(state)) return;

        PreviousState = CurrentState;

        CurrentState.OnExit();
        CurrentState = state;
        CurrentState.OnEnter();
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

        OnPlayerInstantiated?.Invoke(this);

        OnEntityTakeDamage += Player_OnEntityTakeDamage;
        OnEntityDestroyed += Player_OnEntityDestroyed;
    }

    private protected override void OnDeath()
    {
        base.OnDeath();

        OnEntityTakeDamage -= Player_OnEntityTakeDamage;
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
        PlayerDashState.HandleDashTrail();
    }

    private void Player_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject sourceObject)
    {
        CameraShakeManager.Instance.ShakeCamera(5f, 0.25f);
    }

    private void Player_OnEntityDestroyed(Entity destroyedEntity, GameObject killer)
    {
        OnEntityDestroyed -= Player_OnEntityDestroyed;

        OnPlayerDestroyed?.Invoke(this);
    }

    /// <summary>
    /// Accelerates the player's horizontal velocity to the specified speed.
    /// </summary>
    /// <param name="speed">The target speed to accelerate to.</param>
    public void AccelerateToHorizontalSpeed(float speed)
    {
        Vector3 horizontalVelocity = GetHorizontalVelocity();

        horizontalVelocity = Vector3.Lerp(horizontalVelocity, speed * TargetForwardDirection, groundedAcceleration * LocalDeltaTime);

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

        horizontalVelocity = speed * TargetForwardDirection;

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
            velocity.y = PhysicsConfig.GroundedYVelocity;
        }
    }

    private protected override void HandleAirborne()
    {
        if (!IsGrounded)
        {
            if (!PlayerJumpState.IsJumping && !fallVelocityApplied) // falling without jumping
            {
                fallVelocityApplied = true;
                velocity.y = PhysicsConfig.FallingStartingYVelocity;
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
        transform.rotation = Quaternion.Lerp(transform.rotation, TargetForwardRotation, rotationSpeed * LocalDeltaTime);
    }

    /// <summary>
    /// Applies the calculated rotation based off the player's movement and camera to the next movement.
    /// Doesn't actually rotate the player until you call RotateToTargetRotation().
    /// </summary>
    public void ApplyRotationToNextMovement()
    {
        forwardAngleBasedOnCamera = Mathf.Atan2(playerInputReader.MoveDirection.x, playerInputReader.MoveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
        TargetForwardRotation = Quaternion.Euler(0, forwardAngleBasedOnCamera, 0);
        TargetForwardDirection = TargetForwardRotation * Vector3.forward;
    }

    /// <summary>
    /// Applies the given target rotation to the next movement.
    /// Doesn't actually rotate the player until you call RotateToTargetRotation().
    /// </summary>
    /// <param name="targetRotation">The target rotation to apply.</param>
    public void ApplyRotationToNextMovement(Quaternion targetRotation)
    {
        TargetForwardRotation = targetRotation;
        TargetForwardDirection = TargetForwardRotation * Vector3.forward;
    }

    private protected override void EvaluateMovementSpeed()
    {
        MovementSpeed = PlayerSlideState.MovementOnSlopeSpeedModifier * StatusSpeedModifier.GetFloatValue() * SpeedModifier * baseSpeed;
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

    /// <summary>
    /// Replaces a specific clip with the provided one-shot clip.
    /// </summary>
    /// <param name="oneShotClip">The one-shot animation clip to play.</param>
    /// <param name="replacementClipName">The name of the clip to replace.</param>
    public void ReplaceOneShotAnimationClip(AnimationClip oneShotClip, string replacementClipName)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
            return;
        }

        if (oneShotClip == null)
        {
            Debug.LogError("One-shot animation clip is null!");
            return;
        }

        // Get the runtime animator controller
        var runtimeAnimatorController = animator.runtimeAnimatorController;
        if (runtimeAnimatorController == null)
        {
            Debug.LogError("Animator has no RuntimeAnimatorController assigned!");
            return;
        }

        // Create an override controller from the runtime controller
        var overrideController = new AnimatorOverrideController(runtimeAnimatorController);

        // Search for the animation state with the given replacementClipName
        AnimationClip replacementClip = null;
        foreach (var clip in overrideController.animationClips)
        {
            if (clip.name == replacementClipName)
            {
                replacementClip = clip;
                break;
            }
        }

        if (replacementClip == null)
        {
            Debug.LogError($"No animation clip named '{replacementClipName}' found in the Animator!");
            return;
        }

        // Override the animation clip with the one-shot clip
        overrideController[replacementClip] = oneShotClip;

        // Assign the override controller to the animator
        animator.runtimeAnimatorController = overrideController;
    }
}
