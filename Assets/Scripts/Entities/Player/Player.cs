using KBCore.Refs;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Player : Entity
{
    [Header("Player: Debug UI")]
    [SerializeField] private TMP_Text stateText;

    [Header("Player: References")]
    [SerializeField, Self] private CharacterController controller;
    [SerializeField, Self] private PlayerInputReader input;

    [field: Header("Player: Grounded Movement")]
    [field: SerializeField] public float SprintSpeedModifier { get; private set; } = 1.66f;
    [SerializeField] private float groundedAcceleration = 4f;
    private float movementOnSlopeSpeedModifier = 1f;
    public Vector3 MoveDirection => input.MoveDirection;
    private float forwardAngleBasedOnCamera;
    private Quaternion targetForwardRotation = Quaternion.identity;
    private Vector3 targetForwardDirection = Vector3.forward;
    private RaycastHit hitBelow;
    private float hitBelowSlopeAngle;
    private Vector3 velocity;
    public Vector3 Velocity => velocity;

    [Header("Player: Gravity")]
    [SerializeField] private float mass = 1f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private int maxJumpCount = 1;
    private int currentJumpCount;
    
    #region Flags
    [HideInInspector] public bool IsMoving => input.MoveDirection.sqrMagnitude > 0;
    [HideInInspector] public bool IsSprinting;
    [HideInInspector] public bool CanAttack = true;
    [HideInInspector] public bool ApplyRootMotion;
    private bool isJumping;
    #endregion

    [field : Header("Player: Dash")]
    [field: SerializeField] public float DashDuration { get; private set; } = 0.5f;
    [field : SerializeField] public float InitialDashVelocity { get; private set; } = 25f;
    [SerializeField] private float dashDelayDuration = 0.5f;
    [field: SerializeField] public float SprintDurationAfterDash { get; private set; } = 2f;
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
        EntityStaggeredState = new PlayerStaggeredState(this);
        EntityDeathState = new PlayerDeathState(this);
        EntityLaunchState = new PlayerLaunchState(this);
    }
    #endregion

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

        input.Jump.AddListener(Input_HandleJumpInput);
        input.SprintHold.AddListener(Input_HandleSprintInput);
        input.SprintRelease.AddListener(Input_HandleSprintReleaseInput);
        input.Dash.AddListener(Input_HandleDashInput);
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();

        input.Jump.RemoveListener(Input_HandleJumpInput);
        input.SprintHold.RemoveListener(Input_HandleSprintInput);
        input.SprintRelease.RemoveListener(Input_HandleSprintReleaseInput);
        input.Dash.RemoveListener(Input_HandleDashInput);
    }

    private protected override void OnAwake()
    {
        //calls OnAwake from the parent class, Entity
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

        HandleGrounded();
        HandleAirborne();
        HandleDashDelay();
        HandleDashTrail();

        HandleAnimations();

        stateText.text = $"State: {CurrentState.GetType().ToString()}";
    }

    private void OnAnimatorMove()
    {
        if (CurrentState != PlayerAttackState) return;

        if (!ApplyRootMotion) return;

        Vector3 desiredAnimationMovement = animator.deltaPosition;
        //desiredAnimationMovement.y = 0f;

        controller.Move(desiredAnimationMovement);
    }

    private protected override void CheckGrounded()
    {
        base.CheckGrounded();

        //IsGrounded is always false for the first 0.1 seconds in air
        if (inAirTimer > 0f && inAirTimer < 0.1f)
        {
            IsGrounded = false;
            return;
        }

        IsGrounded = GetIsGrounded();
    }

    private bool GetIsGrounded()
    {
        LayerMask mask = LayerMask.GetMask("Entity", "Ground");

        Collider[] hits = Physics.OverlapSphere(transform.position + 9f * controller.radius / 10f * Vector3.up, controller.radius, mask);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private void Input_HandleJumpInput()
    {
        if (!IsGrounded && (currentJumpCount >= maxJumpCount || maxJumpCount == 1)) return;
        if(CurrentState == EntityLaunchState) return;
        if (CurrentState == PlayerSlideState) return;
        if(CurrentState == PlayerChargeState) return;
        if (CurrentState == PlayerAttackState) return;
        if (CurrentState == EntityStaggeredState) return;

        ChangeState(PlayerJumpState);
    }

    private void Input_HandleSprintInput()
    {
        if (CurrentState == PlayerChargeState) return;
        if (CurrentState == PlayerDashState) return;

        IsSprinting = true;
    }

    private void Input_HandleSprintReleaseInput()
    {
        IsSprinting = false;
    }

    private void Input_HandleDashInput()
    {
        if (dashDelayTimer < dashDelayDuration) return;
        if (CurrentState == PlayerChargeState) return;
        if (CurrentState == PlayerDashState) return;
        if (CurrentState == EntityStaggeredState) return;
        if (CurrentState == EntityLaunchState) return;

        input.OnPlayerActionInput?.Invoke(PlayerActions.DASH);
        ChangeState(PlayerDashState);
    }

    public void GroundedMove()
    {
        controller.Move(GetGroundedVelocity() * Time.deltaTime);
    }

    public void AccelerateToSpeed(float speed)
    {
        Vector3 groundedVelocity = GetGroundedVelocity();

        groundedVelocity = Vector3.Lerp(groundedVelocity, speed * targetForwardDirection, groundedAcceleration * Time.deltaTime);

        velocity.x = groundedVelocity.x;
        velocity.z = groundedVelocity.z;
    }

    public void InstantlySetGroundedSpeed(float speed)
    {
        Vector3 groundedVelocity = GetGroundedVelocity();

        groundedVelocity = speed * targetForwardDirection;

        velocity.x = groundedVelocity.x;
        velocity.z = groundedVelocity.z;
    }

    private void HandleGrounded()
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
        }
    }

    private void HandleAirborne()
    {
        if (!IsGrounded)
        {
            if (!isJumping && !fallVelocityApplied) // falling without jumping
            {
                fallVelocityApplied = true;
                velocity.y = PhysicsSettings.FallingStartingYVelocity;

                if (CurrentState != PlayerAttackState && CurrentState != PlayerDashState)
                {
                    ChangeState(PlayerFallState);
                }
            }
            inAirTimer += Time.deltaTime;
            velocity.y += PhysicsSettings.Gravity * Time.deltaTime;
        }
    }

    public void ApplyGravity()
    {
        controller.Move(Time.deltaTime * velocity.y * Vector3.up);
    }

    public void ResetYVelocity()
    {
        velocity.y = 0f;
    }

    public void RotateToTargetRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetForwardRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleDashDelay()
    {
        dashDelayTimer += Time.deltaTime;
    }

    public void ResetDashDelay()
    {
        dashDelayTimer = 0f;
    }

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

        if (IsAbleToSlide())
        {
            Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, normal);

            PlayerSlideState.SetSlideDirection(slideDirection);
            ChangeState(PlayerSlideState);
        }
    }

    public bool IsAbleToSlide()
    {
        if (!IsGrounded) return false;
        if (hitBelow.collider == null) return false;
        if (CurrentState == PlayerAttackState) return false;

        return hitBelowSlopeAngle > controller.slopeLimit;
    }

    public void ApplySlide(Vector3 slideDirection)
    {
        velocity.y = PhysicsSettings.GroundedYVelocity;
        controller.Move(slideDirection * -velocity.y * Time.deltaTime);
    }

    public void ApplyRotationToNextMovement()
    {
        forwardAngleBasedOnCamera = Mathf.Atan2(input.MoveDirection.x, input.MoveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
        targetForwardRotation = Quaternion.Euler(0, forwardAngleBasedOnCamera, 0);
        targetForwardDirection = targetForwardRotation * Vector3.forward;
    }

    private protected override void HandleAnimations()
    {
        base.HandleAnimations();
    }

    private protected override void EvaluateMovementSpeed()
    {
        MovementSpeed = movementOnSlopeSpeedModifier * StatusSpeedModifier * SpeedModifier * baseSpeed;
    }

    public void Jump()
    {
        IsGrounded = false;

        isJumping = true;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * PhysicsSettings.Gravity);
        inAirTimer = 0.01f;

        currentJumpCount++;
    }

    public void DashTrailSetActive(bool b)
    {
        dashTrailObject.SetActive(b);
    }

    private float GetAndSetSlopeSpeedModifierOnAngle(float groundAngle)
    {
        float slopeSpeedModifier = 1f - (0.15f) * groundAngle / controller.slopeLimit;

        if (groundAngle > controller.slopeLimit) slopeSpeedModifier = 0.85f;

        movementOnSlopeSpeedModifier = slopeSpeedModifier;

        return slopeSpeedModifier;
    }

    public Vector3 GetGroundedVelocity()
    {
        return new Vector3(velocity.x, 0f, velocity.z);
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    public float GetMaxSpeed()
    {
        return SprintSpeedModifier * baseSpeed;
    }

    private void HandleDashTrail()
    {
        float maxSpeed = SprintSpeedModifier * baseSpeed;

        DashTrailSetActive(GetGroundedVelocity().magnitude > maxSpeed);
    }

    public void ReplaceComboAnimationClip(AnimationClip newClip)
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);

        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        foreach (AnimationClip currentClip in aoc.animationClips)
        {
            if (currentClip.name == "ComboPlaceholder")
            {
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(currentClip, newClip));
            }
        }

        aoc.ApplyOverrides(anims);

        animator.runtimeAnimatorController = aoc;
    }

    public void SetComboAnimationSpeed(float speed)
    {
        animator.SetFloat("ComboAnimationSpeed", speed);
    }

    public override void Die()
    {
        Destroy(gameObject);
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
