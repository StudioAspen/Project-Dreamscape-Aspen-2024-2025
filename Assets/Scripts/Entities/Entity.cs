using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UPlayable.AnimationMixer;

[RequireComponent(typeof(Animator), typeof(AnimationClipOutput), typeof(AnimatorOutput)),
RequireComponent(typeof(CharacterController))]
public class Entity : MonoBehaviour, IPoolableObject
{
    #region References
    public CharacterController CharacterController { get; protected set; }
    private protected Animator blendTreeAnimator;
    private protected AnimationClipOutput playablesOneShotClipManager;
    private protected AnimatorOutput blendTreeAnimatorManager;

    [field: Header("Entity: References")]
    [field: SerializeField] public GlobalPhysicsConfigSO PhysicsConfig { get; private set; }
    [SerializeField] private protected Transform model;
    #endregion

    #region Health Variables
    [field: Header("Entity: Health")]
    [field: SerializeField] public int CurrentHealth { get; protected set; }
    [field: SerializeField, Tooltip("Max health for entity. Set to 0 for invicibility.")] public Stat MaxHealth { get; protected set; }
    #endregion

    #region Speed Variables
    [Header("Entity: Speed")]
    [SerializeField] private protected float baseSpeed = 3f;
    [SerializeField] private protected float rotationSpeed = 5f;
    [SerializeField] private protected float mass = 1f;
    private protected Vector3 velocity;
    public Vector3 Velocity => velocity;
    public float SpeedModifier { get; protected set; } = 1f;
    [field: SerializeField]public Stat StatusSpeedModifier { get; protected set; } = new Stat(1f);
    public float MovementSpeed { get; protected set; }
    private protected float totalSpeedModifierForAnimation;
    #endregion

    #region Airborne Variables
    [HideInInspector] public bool IsGrounded;
    private protected float inAirTimer;
    private protected bool fallVelocityApplied;
    #endregion

    #region Target Detection Variables
    [Header("Entity: Target Detection")]
    [SerializeField] private protected float targetDetectionRadius = 10f;

    /// <summary>
    /// Gets a list of nearby targets within the specified detection radius.
    /// The entities must be on the "Entity" layer and not on the same team as the entity.
    /// The list is sorted from closest to farthest.
    /// </summary>
    /// <returns>A list of nearby targets.</returns>
    public List<Entity> GetNearbyTargets()
    {
        List<Entity> targets = new List<Entity>();

        Collider[] hits = Physics.OverlapSphere(transform.position, targetDetectionRadius, LayerMask.GetMask("Entity"));
        if (hits == null) return targets;
        if (hits.Length == 0) return targets;

        foreach (Collider hit in hits)
        {
            Entity potentialTarget = hit.GetComponent<Entity>();
            if (potentialTarget == null) continue;
            if (potentialTarget.Team == Team) continue;
            targets.Add(potentialTarget);
        }

        return targets.OrderBy(target => Vector3.SqrMagnitude(transform.position - target.transform.position)).ToList();
    }

    /// <summary>
    /// Gets a list of nearby hostile entities within the specified radius.
    /// The entities must be on the "Entity" layer and not on the same team as the entity.
    /// The list is sorted from closest to farthest.
    /// </summary>
    /// <param name="radius">The radius within which to search for nearby entities.</param>
    /// <param name="willIncludeDying">Whether to include dying entities.</param>
    /// <returns>A list of nearby entities.</returns>
    public List<Entity> GetNearbyHostileEntities(float radius, bool willIncludeDying = true)
    {
        List<Entity> targets = new List<Entity>();

        Collider[] hits = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Entity"));
        if (hits == null) return targets;
        if (hits.Length == 0) return targets;

        foreach (Collider hit in hits)
        {
            Entity potentialTarget = hit.GetComponent<Entity>();
            if (potentialTarget == null) continue;
            if (potentialTarget.Team == Team) continue;
            if (!willIncludeDying && potentialTarget.CurrentState == potentialTarget.EntityDeathState) continue;
            targets.Add(potentialTarget);
        }

        return targets.OrderBy(target => Vector3.SqrMagnitude(transform.position - target.transform.position)).ToList();
    }

    /// <summary>
    /// Gets a list of nearby hostile entities of a specific type within the specified radius.
    /// The entities must be on the "Entity" layer and not on the same team as the entity.
    /// The list is sorted from closest to farthest.
    /// </summary>
    /// <typeparam name="T">The type of entities to retrieve.</typeparam>
    /// <param name="radius">The radius within which to search for nearby entities.</param>
    /// <param name="willIncludeDying">Whether to include dying entities.</param>
    /// <returns>A list of nearby entities of the specified type.</returns>
    public List<T> GetNearbyHostileEntitiesByType<T>(float radius, bool willIncludeDying = true) where T : Entity
    {
        List<T> targets = new List<T>();

        Collider[] hits = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Entity"));
        if (hits == null) return targets;
        if (hits.Length == 0) return targets;

        foreach (Collider hit in hits)
        {
            Entity potentialTarget = hit.GetComponent<Entity>();
            if (potentialTarget == null) continue;
            if (potentialTarget.GetType() != typeof(T)) continue;
            if (potentialTarget.Team == Team) continue;
            if (!willIncludeDying && potentialTarget.CurrentState == potentialTarget.EntityDeathState) continue;

            T potentialTargetT = potentialTarget as T;

            targets.Add(potentialTargetT);
        }

        return targets.OrderBy(target => Vector3.SqrMagnitude(transform.position - target.transform.position)).ToList();
    }
    #endregion

    #region Team Variables
    [field: Header("Entity: Team")]
    [field: SerializeField] public int Team { get; private set; }

    /// <summary>
    /// Changes the team of the entity to the specified new team.
    /// Equal teams cannot damage each other.
    /// </summary>
    /// <param name="newTeam">The new team to assign to the entity.</param>
    public void ChangeTeam(int newTeam)
    {
        Team = newTeam;
    }
    #endregion

    #region Attack Variables
    [field: Header("Entity: Attack")]
    [field: SerializeField] public Vector2Int BaseDamageRange { get; protected set; } = new Vector2Int(10, 15);
    [field: SerializeField] public Stat DamageModifier { get; protected set; } = new Stat(1f);
    public Stat DebuffSpeedMultiplier { get; protected set; } = new Stat(1f);
    [HideInInspector] public bool UseRootMotion;

    /// <summary>
    /// Calculates the damage based on the given multiplier.
    /// </summary>
    /// <param name="multiplier">The multiplier of the damage range to calculate.</param>
    /// <returns>The calculated damage value.</returns>
    public int CalculateDamage(float multiplier)
    {
        Vector2Int modifiedDamageRange = Vector2Int.RoundToInt(
                multiplier * DamageModifier.GetFloatValue() * new Vector2(BaseDamageRange.x, BaseDamageRange.y)
            );

        return UnityEngine.Random.Range(modifiedDamageRange.x, modifiedDamageRange.y);
    }
    #endregion

    #region Movement Events
    /// <summary>
    /// Action that is invoked when the entity becomes grounded.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Vector3 groundPoint</c>: Where you got grounded.</description></item>
    /// </list>
    /// </remarks>
    public Action<Vector3> OnGrounded = delegate { };
    /// <summary>
    /// Action that is invoked when the entity becomes airborne.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Vector3 groundPoint</c>: Where you left the ground.</description></item>
    /// </list>
    /// </remarks>
    public Action<Vector3> OnAirborne = delegate { };
    private bool prevIsGrounded;

    /// <summary>
    /// Invokes the vertical movement events based on the current grounded state.
    /// </summary>
    private protected void InvokeVerticalMovementEvents()
    {
        if (prevIsGrounded != IsGrounded)
        {
            if (IsGrounded)
            {
                OnGrounded?.Invoke(transform.position);
            }
            else
            {
                OnAirborne?.Invoke(transform.position);
            }
            prevIsGrounded = IsGrounded;
        }
    }
    #endregion

    #region Combat Events
    /// <summary>
    /// Action that is invoked when the entity deals damage to another entity.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Entity attacker</c>: The attacker entity</description></item>
    /// <item><description><c>Entity victim</c>: The victim entity that got hit</description></item>
    /// <item><description><c>Vector3 hitPoint</c>: Where the hit was</description></item>
    /// <item><description><c>int damage</c>: How much damage the hit did</description></item>
    /// </list>
    /// </remarks>
    public Action<Entity, Entity, Vector3, int> OnEntityDealDamage = delegate { };
    /// <summary>
    /// Action that is invoked when the entity gets healed.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Entity healedEntity</c>: The entity that got healed</description></item>
    /// <item><description><c>int healValue</c>: The heal amount</description></item>
    /// </list>
    /// </remarks>
    public Action<Entity, int> OnEntityHeal = delegate { };
    /// <summary>
    /// Action that is invoked when the entity takes damage.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>int damage</c>: The amount of damage taken.</description></item>
    /// <item><description><c>Vector3 hitPoint</c>: The point where the entity was hit.</description></item>
    /// <item><description><c>GameObject source</c>: The source of the damage.</description></item>
    /// </list>
    /// </remarks>
    public Action<int, Vector3, GameObject> OnEntityTakeDamage = delegate { };
    /// <summary>
    /// Action that is invoked when the entity enters the death state.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>GameObject killer</c>: The killer source.</description></item>
    /// </list>
    /// </remarks>
    public Action<GameObject> OnEntityDeath = delegate { };
    /// <summary>
    /// Action that is invoked right before the entity is destroyed after the death state. If the entity is pooled, this invokes right before it gets released.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Entity destroyedEntity</c>: The entity that was destroyed.</description></item>
    /// <item><description><c>GameObject killer</c>: The killer source.</description></item>
    /// </list>
    /// </remarks>
    public Action<Entity, GameObject> OnEntityDestroyed = delegate { };
    /// <summary>
    /// Action that is invoked when the entity kills another entity.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Entity victim</c>: The victim entity that was killed.</description></item>
    /// </list>
    /// </remarks>
    public Action<Entity> OnKillEntity = delegate { }; // passes the victim entity
    private protected GameObject lastHitSource;
    #endregion

    #region Local Time Scale
    [field: Header("Local Time Scale")]
    [field: SerializeField] public Stat LocalTimeScale { get; protected set; } = new Stat(1f);
    public float LocalDeltaTime => Time.deltaTime * LocalTimeScale.GetFloatValue();
    #endregion

    #region Scale
    public Stat SizeScale { get; protected set; } = new Stat(1f);
    #endregion

    #region Pooling Variables
    private ObjectPool<GameObject> pool;

    /// <summary>
    /// Sets the object pool for the entity.
    /// Must be used if the entity is pooled.
    /// </summary>
    /// <param name="objectPool">The object pool to set.</param>
    public void SetObjectPool(ObjectPool<GameObject> objectPool)
    {
        pool = objectPool;
    }
    #endregion

    #region States
    public EntityBaseState CurrentState { get; protected set; }
    public EntityBaseState PreviousState { get; protected set; }
    public EntityBaseState DefaultState { get; protected set; }

    [field: Header("Entity: States")]
    [field: SerializeField] public EntityEmptyState EntityEmptyState { get; protected set; }
    [field: SerializeField] public EntityStaggeredState EntityStaggeredState { get; protected set; }
    [field: SerializeField] public EntityDeathState EntityDeathState { get; protected set; }
    [field: SerializeField] public EntityLaunchState EntityLaunchState { get; protected set; }
    [field: SerializeField] public EntityStunnedState EntityStunnedState { get; protected set; }
    [field: SerializeField] public EntitySpawnState EntitySpawnState { get; protected set; }

    /// <summary>
    /// Initializes the states for the entity.
    /// Override this function to add more states to the entity.
    /// Entity states can be initialized as inherited versions of those states.
    /// </summary>
    private protected virtual void InitializeStates()
    {
        //makes new state scripts for the entity to use
        EntityEmptyState.Init(this);
        EntityDeathState.Init(this);
        EntityLaunchState.Init(this);
        EntityStaggeredState.Init(this);
        EntityStunnedState.Init(this);
        EntitySpawnState.Init(this);
    }

    /// <summary>
    /// Sets the start state of the entity.
    /// </summary>
    /// <param name="state">The start state to set.</param>
    private protected void SetStartState(EntityBaseState state)
    {
        CurrentState = state;
        CurrentState.OnEnter();
    }

    /// <summary>
    /// Sets the default state of the entity.
    /// </summary>
    /// <param name="state">The default state to set.</param>
    private protected void SetDefaultState(EntityBaseState state)
    {
        DefaultState = state;
    }

    /// <summary>
    /// Change the state machine state to the specified new state.
    /// Force changing is used to change the state even when the current state is the same and is set to false by default.
    /// Override this function if you want to add custom state changing logic.
    /// </summary>
    /// <param name="state">The new state to change to.</param>
    /// <param name="willForceChange">Whether to change the state even when the current state is the same.</param>
    public virtual void ChangeState(EntityBaseState state, bool willForceChange = false)
    {
        if (CurrentState == EntityDeathState) return;
        if (!willForceChange && CurrentState == state) return;

        PreviousState = CurrentState;

        CurrentState.OnExit();
        CurrentState = state;
        CurrentState.OnEnter();
    }
    #endregion

    private void OnValidate()
    {
        // For the gizmos to stop throwing errors before pressing play
        CharacterController = GetComponent<CharacterController>(); 
    }

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        blendTreeAnimator = GetComponent<Animator>();
        playablesOneShotClipManager = GetComponent<AnimationClipOutput>();
        blendTreeAnimatorManager = GetComponent<AnimatorOutput>();

        //We have to make custom OnAwake and OnStart functions
        //because you cannot override the regular Awake() and Start() methods
        //using inheritance
        OnAwake();
    }

    /// <summary>
    /// This method is called during the Awake phase of the MonoBehaviour lifecycle.
    /// It initializes the states for the entity.
    /// Override this function for custom Awake logic.
    /// </summary>
    private protected virtual void OnAwake()
    {
        InitializeStates();
    }

    private void OnEnable()
    {
        OnOnEnable();
    }

    /// <summary>
    /// This method is called during the OnEnable phase of the MonoBehaviour lifecycle.
    /// It sets the start state of the entity and sets the entity to max health.
    /// Override this function for custom OnEnable logic.
    /// </summary>
    private protected virtual void OnOnEnable()
    {
        velocity = Vector3.zero;

        IsGrounded = false;
        prevIsGrounded = true;
        inAirTimer = 0f;
        fallVelocityApplied = false;

        lastHitSource = null;

        CurrentHealth = MaxHealth.GetIntValue();

        IgnoreOtherEntityCollisions(false);

        SetStartState(EntityEmptyState);
    }

    private void OnDisable()
    {
        OnOnDisable();
    }

    /// <summary>
    /// This method is called during the OnDisable phase of the MonoBehaviour lifecycle.
    /// Override this function for custom OnDisable logic.
    /// </summary>
    private protected virtual void OnOnDisable()
    {
        CurrentState?.OnExit();

        Warp(new Vector3(0f, 10000f, 0f));
    }

    private void Start()
    {
        OnStart();
    }

    /// <summary>
    /// This method is called during the Start phase of the MonoBehaviour lifecycle.
    /// It sets the default state of the entity and ignores the entity's own colliders.
    /// Override this function for custom Start logic.
    /// </summary>
    private protected virtual void OnStart()
    {
        SetDefaultState(EntityEmptyState);

        IgnoreMyOwnColliders();
    }

    private void Update()
    {
        HandleSize();

        OnUpdate();
    }

    /// <summary>
    /// This method is called during the Update phase of the MonoBehaviour lifecycle.
    /// It updates the current state of the entity and checks if the entity is grounded.
    /// Override this function for custom Update logic.
    /// </summary>
    private protected virtual void OnUpdate()
    {
        //if CurrentState isn't null, run it's Update function
        //the states are regular C# scripts because if we did another Monobehavior, it'd add a second call to Update which isn't really necessary n takes extra resources..
        CurrentState?.OnUpdate();

        HandleBlendTreeAnimation();
        EvaluateMovementSpeed();

        HandleGrounded();
        HandleAirborne();
        HandleVerticalVelocity();

        SlideOffOtherEntities();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }

    /// <summary>
    /// This method is called during the FixedUpdate phase of the MonoBehaviour lifecycle.
    /// It fixed updates the current state of the entity.
    /// Override this function for custom FixedUpdate logic.
    /// </summary>
    private protected virtual void OnFixedUpdate()
    {
        CheckGrounded();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        CurrentState?.OnOnControllerColliderHit(hit);
    }

    /// <summary>
    /// Handles the collision enter event for the entity.
    /// Override this function if you want to add custom collision enter logic.
    /// </summary>
    /// <param name="collision">The collision data.</param>
    private protected virtual void OnOnControllerColliderHit(ControllerColliderHit hit)
    {
        CurrentState?.OnOnControllerColliderHit(hit);
    }

    private void OnAnimatorMove()
    {
        OnOnAnimatorMove();
    }

    /// <summary>
    /// Handles the OnAnimatorMove event to apply root motion to the character controller.
    /// Override this function if you want to add custom root motion logic.
    /// </summary>
    private protected virtual void OnOnAnimatorMove()
    {
        if (!UseRootMotion) return;

        float modelScale = model.localScale.x;
        Vector3 desiredAnimationMovement = modelScale * blendTreeAnimator.deltaPosition;
        desiredAnimationMovement.y = 0f;

        CharacterController.Move(desiredAnimationMovement);
    }
    private void OnDrawGizmos()
    {
        CurrentState?.OnDrawGizmos();

        OnOnDrawGizmos();
    }

    /// <summary>
    /// Handles the drawing of Gizmos in the scene view.
    /// Override this function if you want to add custom gizmo drawing logic.
    /// </summary>
    private protected virtual void OnOnDrawGizmos()
    {

    }

    /// <summary>
    /// Handles the IsGrounded bool for the entity. Override this method to add custom grounded checks.
    /// Also invokes the events for OnGrounded and OnAirborne.
    /// </summary>
    private protected virtual void CheckGrounded()
    {
        InvokeVerticalMovementEvents();

        if (velocity.y > 0f)
        {
            IsGrounded = false;
            return;
        }

        //IsGrounded is always false for the first 0.1 seconds in air
        if (inAirTimer > 0f && inAirTimer < 0.1f)
        {
            IsGrounded = false;
            return;
        }

        IsGrounded = GetIsGrounded();
    }

    /// <summary>
    /// Checks if the entity is grounded.
    /// </summary>
    /// <returns>True if the entity is grounded, false otherwise.</returns>
    private protected bool GetIsGrounded()
    {
        return Physics.CheckSphere(transform.position + 9f * CharacterController.radius / 10f * Vector3.up, CharacterController.radius, LayerMask.GetMask("Ground"));
    }

    /// <summary>
    /// Gets the list of RaycastHit objects below the entity within a specified distance and on specified layers.
    /// </summary>
    /// <param name="mask">The layer mask to filter the raycast hits.</param>
    /// <param name="distance">The maximum distance to perform the raycast.</param>
    /// <returns>A list of RaycastHit objects representing the hits below the entity.</returns>
    public List<RaycastHit> GetHitsBelowEntity(LayerMask mask, float distance)
    {
        RaycastHit[] hits = Physics.SphereCastAll(GetColliderCenterPosition(), CharacterController.radius, Vector3.down, distance + Vector3.Distance(GetColliderCenterPosition(), transform.position), mask);
        List<RaycastHit> result = new List<RaycastHit>();

        if (hits == null) return result;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != gameObject)
            {
                result.Add(hit);
            }
        }

        return result;
    }

    /// <summary>
    /// Handles the behavior when the entity is grounded.
    /// Override this function if you want to add custom grounded logic.
    /// </summary>
    private protected virtual void HandleGrounded()
    {
        if (IsGrounded)
        {
            inAirTimer = 0f;
            fallVelocityApplied = false;

            velocity.y = PhysicsConfig.GroundedYVelocity;
        }
    }

    /// <summary>
    /// Handles the behavior when the entity is airborne.
    /// Override this function if you want to add custom grounded logic.
    /// </summary>
    private protected virtual void HandleAirborne()
    {
        if (!IsGrounded)
        {
            inAirTimer += LocalDeltaTime;
        }
    }

    /// <summary>
    /// Handles the vertical velocity of the entity.
    /// </summary>
    private protected virtual void HandleVerticalVelocity()
    {
        if (!IsGrounded)
        {
            velocity.y += LocalDeltaTime * PhysicsConfig.Gravity;
        }
    }

    /// <summary>
    /// Applies gravity to the entity if it is not grounded.
    /// Override this function if you want to add custom gravity logic.
    /// </summary>
    public virtual void ApplyGravity()
    {
        CharacterController.Move(LocalDeltaTime * velocity.y * Vector3.up);
    }

    /// <summary>
    /// Allows the entity to change from grounded to airborne state by setting the inAirTimer to a small value greater than 0.
    /// Doesn't work if the entity is already airborne.
    /// </summary>
    public void AllowChangeFromGroundedToAirborne()
    {
        // If the entity is not grounded, then it is already airborne
        if (!IsGrounded) return;

        inAirTimer = 0.01f;
    }

    /// <summary>
    /// Slides off other entities if there are any below the entity.
    /// </summary>
    private protected virtual void SlideOffOtherEntities()
    {
        if(CurrentState == EntityDeathState) return;
        if(IsIgnoringOtherEntityCollisions()) return;

        float distanceToCheckBelow = 0.25f;

        int validHitEntitiesBelow = 0;

        foreach(RaycastHit hit in GetHitsBelowEntity(LayerMask.GetMask("Entity"), distanceToCheckBelow))
        {
            if(hit.collider.TryGetComponent(out Entity hitEntity))
            {
                if(hitEntity.CurrentState == hitEntity.EntityDeathState) continue; // Filter out dead entities
                if (hitEntity.IsIgnoringOtherEntityCollisions()) continue;

                validHitEntitiesBelow++;
            }
        }

        if (validHitEntitiesBelow > 0)
        {
            ForceUpdateHorizontalVelocity(transform.forward, 3f);
            ApplyHorizontalVelocity();
        }
    }

    /// <summary>
    /// Ignores or includes collisions with other entities.
    /// </summary>
    /// <param name="willIgnore">True to ignore collisions with other entities, false to include collisions.</param>
    public void IgnoreOtherEntityCollisions(bool willIgnore = true)
    {
        CharacterController.excludeLayers = willIgnore ? LayerMask.GetMask("Entity") : 0;
    }

    /// <summary>
    /// Determines whether the entity is currently ignoring collisions with other entities.
    /// </summary>
    /// <returns>Whether the entity is currently ignoring collisions with other entities</returns>
    public bool IsIgnoringOtherEntityCollisions()
    {
        return CharacterController.excludeLayers == LayerMask.GetMask("Entity");
    }

    /// <summary>
    /// Resets the Y velocity of the entity to zero.
    /// </summary>
    public void ResetYVelocity()
    {
        velocity.y = 0f;
    }

    /// <summary>
    /// Gets the horizontal velocity of the entity.
    /// </summary>
    /// <returns>The grounded velocity.</returns>
    public Vector3 GetHorizontalVelocity()
    {
        return new Vector3(velocity.x, 0f, velocity.z);
    }

    /// <summary>
    /// Sets the velocity of the entity.
    /// </summary>
    /// <param name="newVelocity">The new velocity to set.</param>
    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    /// <summary>
    /// Moves the entity in its horizontal velocity direction.
    /// Override this function if you want to add custom horizontal movement logic.
    /// </summary>
    public virtual void ApplyHorizontalVelocity()
    {
        CharacterController.Move(GetHorizontalVelocity() * LocalDeltaTime);
    }

    /// <summary>
    /// Updates the horizontal velocity of the entity based on the given direction.
    /// </summary>
    /// <param name="direction">The direction of movement.</param>
    public void UpdateHorizontalVelocity(Vector3 direction)
    {
        Vector3 horizontalVelocity = MovementSpeed * new Vector3(direction.x, 0f, direction.z).normalized;

        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
    }

    /// <summary>
    /// Forces an update to the horizontal velocity of the entity, ignoring the current speed modifier.
    /// </summary>
    /// <param name="direction">The direction of the velocity.</param>
    /// <param name="speed">The speed of the velocity.</param>
    public void ForceUpdateHorizontalVelocity(Vector3 direction, float speed)
    {
        Vector3 groundedVelocity = speed * new Vector3(direction.x, 0f, direction.z).normalized;

        velocity.x = groundedVelocity.x;
        velocity.z = groundedVelocity.z;
    }

    /// <summary>
    /// Handles the blend tree animation of the entity.
    /// Sets the MovementSpeed parameter for the FlatMovement blend tree
    /// </summary>
    private protected virtual void HandleBlendTreeAnimation()
    {
        totalSpeedModifierForAnimation = Mathf.Lerp(totalSpeedModifierForAnimation, SpeedModifier, 7.5f * LocalDeltaTime);

        blendTreeAnimator.SetFloat("MovementSpeed", totalSpeedModifierForAnimation);
        blendTreeAnimator.speed = LocalTimeScale.GetFloatValue();
    }

    /// <summary>
    /// Plays a one shot animation using Playables API. If you want to return to the default blend tree, you must call PlayDefaultAnimation().
    /// </summary>
    /// <param name="animationClip">The clip to play.</param>
    /// <param name="clipDuration">The duration you want to force the clip into. The default is the regular clip length.</param>
    /// <param name="transitionDuration">The fade duration.</param>
    public void PlayOneShotAnimation(AnimationClip animationClip, float clipDuration = 0f, float transitionDuration = 0.1f)
    {
        if(animationClip == null)
        {
            Debug.LogWarning("Cant play null one shot animation");
            return;
        }

        if(playablesOneShotClipManager == null)
        {
            return;
        }

        if (!playablesOneShotClipManager.IsReady()) return;

        playablesOneShotClipManager.ToClip = animationClip;

        float speed = (clipDuration <= 0f) ? 1f : animationClip.length / clipDuration;
        speed = speed * LocalTimeScale.GetFloatValue();
        playablesOneShotClipManager.SetSpeed(speed);

        playablesOneShotClipManager.SetTransitionDuration(transitionDuration / LocalTimeScale.GetFloatValue());

        playablesOneShotClipManager.Play();
    }

    /// <summary>
    /// Plays the default blend tree animation
    /// </summary>
    /// <param name="transitionDuration">The fade duration to the animation</param>
    public void PlayDefaultAnimation(float transitionDuration = 0.1f)
    {
        if (blendTreeAnimatorManager == null) return;
        if (blendTreeAnimatorManager.AnimationControll == null) return;
        if (!blendTreeAnimatorManager.IsReady()) return;

        blendTreeAnimatorManager.SetSpeed(LocalTimeScale.GetFloatValue());
        blendTreeAnimatorManager.SetTransitionDuration(transitionDuration / LocalTimeScale.GetFloatValue());
        blendTreeAnimatorManager.Play();
    }

    /// <summary>
    /// Handles the death logic for the entity by invoking the OnEntityDeath event and attempting to notify the killer.
    /// Also changes the state to the death state.
    /// Override this function if you want to add custom death logic.
    /// </summary>
    private protected virtual void OnDeath()
    {
        ChangeState(EntityDeathState);

        AttemptToNotifyKiller();

        OnEntityDeath?.Invoke(lastHitSource);
    }

    /// <summary>
    /// Fired by the entity's death animation event once the death animation has finished playing.
    /// Releases the entity back to the object pool or destroys it if there is no object pool.
    /// Override this function if you want to add custom logic for how the entity is destroyed.
    /// </summary>
    public virtual void Die()
    {
        OnEntityDestroyed?.Invoke(this, lastHitSource);

        if (pool == null)
        {
            Destroy(gameObject);
            return;
        }

        pool.Release(gameObject);
    }

    /// <summary>
    /// Attempts to notify the entity that killed this entity by checking if the last hit source is an entity.
    /// If this fails, then the source that killed this entity is not an entity and cannot be notified.
    /// Override this function if you want to add custom logic for notifying the killer.
    /// </summary>
    private protected virtual void AttemptToNotifyKiller()
    {
        if (lastHitSource == null) return;

        if (lastHitSource.TryGetComponent(out Entity killer))
        {
            killer.OnKill(this);
        }
    }

    /// <summary>
    /// Changes the entity's scale based on the SizeScale value
    /// </summary>
    private void HandleSize()
    {
        transform.localScale = SizeScale.GetFloatValue() * Vector3.one;
    }

    /// <summary>
    /// Sets the base maximum health of the entity and optionally heals it to full health. Do not use this for buffs.
    /// </summary>
    /// <param name="newMaxHealth">The new maximum health value.</param>
    /// <param name="willHealToFull">Whether the entity will be healed to full health.</param>
    public void SetBaseMaxHealth(int newMaxHealth, bool willHealToFull)
    {
        MaxHealth.SetBaseValue(newMaxHealth);

        if (willHealToFull) CurrentHealth = MaxHealth.GetIntValue();
    }

    /// <summary>
    /// Entity becomes staggered on hit and cannot take damage if it is in the death state.
    /// Takes damage and updates the entity's health while checking to see if the entity's health reaches below zero.
    /// Attempts to spawn hit numbers at the hit point and invokes the OnEntityTakeDamage event.
    /// Entity will be invicible if max health is set to 0.
    /// Override this function if you want to add custom damage taking logic.
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    /// <param name="hitPoint">The point where the entity was hit.</param>
    /// <param name="source">The source of the damage.</param>
    /// <param name="willTryStagger">If the instance of damage will try to stagger.</param>
    public virtual void TakeDamage(int damage, Vector3 hitPoint, GameObject source, bool willTryStagger = true)
    {
        if (CurrentState == EntityDeathState) return;

        if(willTryStagger) TryChangeStaggeredState();

        AttemptToSpawnHitNumbers(damage, hitPoint, Color.red);

        CurrentHealth -= damage;

        OnEntityTakeDamage?.Invoke(damage, hitPoint, source);

        lastHitSource = source;

        //after calculating current health, check if the player has taken enough damage to die
        if (CurrentHealth <= 0 && MaxHealth.GetIntValue() > 0)
        {
            CurrentHealth = 0;
            OnDeath();
        }
    }

    /// <summary>
    /// Deals damage to the specified entity.
    /// </summary>
    /// <param name="victim">The entity to deal damage to.</param>
    /// <param name="damage">The amount of damage to deal.</param>
    /// <param name="hitPoint">The point where the damage was dealt.</param>
    /// <param name="willTryStagger">Whether to try to stagger the victim.</param>
    public virtual void DealDamageToOtherEntity(Entity victim, int damage, Vector3 hitPoint, bool willTryStagger = true)
    {
        OnEntityDealDamage?.Invoke(this, victim, hitPoint, damage);

        victim.TakeDamage(damage, hitPoint, gameObject, willTryStagger);
    }

    /// <summary>
    /// Determines if the entity will die from the given damage.
    /// </summary>
    /// <param name="damage">The amount of damage.</param>
    /// <returns>True if the entity will die, false otherwise.</returns>
    public virtual bool WillDieFromDamage(int damage)
    {
        return MaxHealth.GetIntValue() > 0 && CurrentHealth - damage <= 0;
    }

    /// <summary>
    /// Tries to change the state of the entity to the staggered state.
    /// If the current state is already the fling state, it does nothing.
    /// </summary>
    private protected virtual void TryChangeStaggeredState()
    {
        if (CurrentState == EntityLaunchState) return;
        if (CurrentState == EntityStunnedState) return;
        if (!CanBeStaggered()) return;

        ChangeState(EntityStaggeredState, true);
    }

    /// <summary>
    /// Determines if the entity can get staggered.
    /// Override this method to prevent stagger depending on current state.
    /// </summary>
    /// <returns>Whether the entity can be staggered</returns>
    public virtual bool CanBeStaggered()
    {
        return true;
    }

    /// <summary>
    /// Attempts to spawn hit numbers at the hit point with the specified damage.
    /// Fails if the HitNumberPooler is not found.
    /// </summary>
    /// <param name="damage">The amount of damage to display.</param>
    /// <param name="hitPoint">The point where the entity was hit.</param>
    /// <param name="color">The color of the text.</param>
    private protected void AttemptToSpawnHitNumbers(int damage, Vector3 hitPoint, Color color)
    {
        if (damage <= 0) return;

        ObjectPooler spawner = GameObject.Find("HitNumberPooler").GetComponent<ObjectPooler>();
        if (spawner == null) return;

        HitNumbers hitNumber = spawner.SpawnObject<HitNumbers>();

        Vector3 hitNumberFloatDirection = hitPoint - transform.position;

        hitNumber.ActivateHitNumberText(damage, GetRandomPositionOnCollider(), hitNumberFloatDirection.normalized, color);
    }

    /// <summary>
    /// Increases the current health of the entity by the specified amount.
    /// </summary>
    /// <param name="health">The amount of health to add.</param>
    public void Heal(int health)
    {
        OnEntityHeal?.Invoke(this, health);

        CurrentHealth += health;
        if(CurrentHealth > MaxHealth.GetIntValue()) CurrentHealth = MaxHealth.GetIntValue();

        AttemptToSpawnHitNumbers(health, gameObject.transform.position + Vector3.up, Color.green);
    }

    /// <summary>
    /// Heals the entity to full.
    /// </summary>
    /// <param name="willSpawnHitNumbers">Whether to spawn hit numbers or not.</param>
    public void HealToFull(bool willSpawnHitNumbers = true)
    {
        if(willSpawnHitNumbers) AttemptToSpawnHitNumbers(MaxHealth.GetIntValue() - CurrentHealth, gameObject.transform.position + Vector3.up, Color.green);

        CurrentHealth = MaxHealth.GetIntValue();
    }

    /// <summary>
    /// Kills the entity by dealing maximum damage to itself.
    /// Doesn't work if the entity has max health set to 0.
    /// </summary>
    /// /// <param name="sourceObject">The source object that killed the entity.</param>
    public void Kill(GameObject sourceObject)
    {
        TakeDamage(int.MaxValue, transform.position, sourceObject);
    }

    /// <summary>
    /// Warps the entity to the specified position accounting for character controller physics issues.
    /// </summary>
    /// <param name="newPosition">The new position to warp to.</param>
    public void Warp(Vector3 newPosition)
    {
        transform.position = newPosition;
        Physics.SyncTransforms();
    }

    /// <summary>
    /// Sets the speed modifier of the entity. The speed modifier is a multiplier that affects the entity's base movement speed.
    /// </summary>
    /// <param name="speed">The speed modifier to set.</param>
    public void SetSpeedModifier(float speed)
    {
        SpeedModifier = speed;
    }

    /// <summary>
    /// Invoked when this entity kills another entity.
    /// Override this function if you want to add custom on kill logic.
    /// </summary>
    /// <param name="entity">The entity that was killed.</param>
    public virtual void OnKill(Entity entity)
    {
        OnKillEntity?.Invoke(entity);
    }

    /// <summary>
    /// Evaluates the movement speed of the entity based on the status speed modifier, speed modifier, and base speed.
    /// </summary>
    private protected virtual void EvaluateMovementSpeed()
    {
        MovementSpeed = StatusSpeedModifier.GetFloatValue() * SpeedModifier * baseSpeed;
    }

    /// <summary>
    /// Rotates the entity to face the specified target position with a the default rotationSpeed.
    /// Must be called in Update to work.
    /// Returns the target rotation of the entity.
    /// </summary>
    /// <param name="target">The position to look at.</param>
    public Quaternion LookAt(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * LocalDeltaTime);

        return targetRotation;
    }

    /// <summary>
    /// Rotates the entity to face the specified target with a given speed.
    /// Must be called in Update to work.
    /// Returns the target rotation of the entity.
    /// </summary>
    /// <param name="target">The target position to look at.</param>
    /// <param name="speed">The rotation speed.</param>
    /// <returns>The target rotation.</returns>
    public Quaternion LookAt(Vector3 target, float speed)
    {
        Vector3 dir = target - transform.position;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * LocalDeltaTime);

        return targetRotation;
    }

    /// <summary>
    /// Calculates the distance between the entity and the specified target position.
    /// </summary>
    /// <param name="target">The target position to calculate the distance to.</param>
    /// <returns>The distance between the entity and the target position.</returns>
    public float Distance(Vector3 target)
    {
        return Vector3.Distance(target, transform.position);
    }

    /// <summary>
    /// Calculates the distance between the entity and the specified target entity.
    /// </summary>
    /// <param name="entity">The target entity to calculate the distance to.</param>
    /// <returns>The distance between the entity and the target entity.</returns>
    public float Distance(Entity entity)
    {
        return Vector3.Distance(entity.transform.position, transform.position);
    }

    /// <summary>
    /// Checks if the entity is close to a specified point within a given error margin.
    /// Error is defaulted to 0.05f.
    /// </summary>
    /// <param name="point">The point to check against.</param>
    /// <param name="error">The maximum allowed distance from the point.</param>
    /// <returns>True if the entity is close to the point within the error margin, false otherwise.</returns>
    public bool CloseToPoint(Vector3 point, float error = 0.05f)
    {
        return Distance(point) < error;
    }

    /// <summary>
    /// Determines if the current entity is blocked from another entity by performing a raycast between their positions.
    /// Blockers are on the layers that aren't "Entity", "Damageable Entity", and "Damage Collider".
    /// </summary>
    /// <param name="entity">The entity to check if blocked from.</param>
    /// <returns>True if the current entity is blocked from the specified entity, false otherwise.</returns>
    public bool IsBlockedFromEntity(Entity entity)
    {
        LayerMask ignoreLayers = ~LayerMask.GetMask("Entity", "Damageable Entity", "Damage Collider");

        RaycastHit hit;
        Physics.Raycast(transform.position, entity.transform.position - transform.position, out hit, Distance(entity), ignoreLayers);

        if (hit.collider == null)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Ignores collisions between the colliders attached to the entity and its child objects.
    /// Currently disabled because body parts have their collisions disabled.
    /// Will be re-enabled once we add large bosses.
    /// </summary>
    private void IgnoreMyOwnColliders()
    {
        Collider baseCollider = CharacterController;
        Collider[] damageableColliders = GetComponentsInChildren<Collider>();
        List<Collider> ignoreColliders = new List<Collider>();

        foreach (Collider collider in damageableColliders)
        {
            ignoreColliders.Add(collider);
        }

        ignoreColliders.Add(baseCollider);

        foreach (Collider c1 in ignoreColliders)
        {
            foreach (Collider c2 in ignoreColliders)
            {
                Physics.IgnoreCollision(c1, c2, true);
            }
        }
    }

    /// <summary>
    /// Gets the center position of the collider attached to the entity.
    /// </summary>
    /// <returns>The center position of the collider.</returns>
    public Vector3 GetColliderCenterPosition()
    {
        return CharacterController.bounds.center;
    }

    /// <summary>
    /// Gets the top point of the entity.
    /// </summary>
    /// <returns>The the top point of the entity.</returns>
    public Vector3 GetEntityTopPosition()
    {
        return transform.position + 2f * (GetColliderCenterPosition() - transform.position);
    }

    /// <summary>
    /// Gets the largest size of the collider attached to the entity.
    /// </summary>
    /// <returns>The largest size of the collider.</returns>
    public float GetColliderLargestSize()
    {
        Vector3 size = CharacterController.bounds.size;

        return Mathf.Max(size.x, size.y, size.z);
    }

    /// <summary>
    /// Returns a random position on the collider of the entity.
    /// </summary>
    /// <returns>The random position on the collider.</returns>
    public Vector3 GetRandomPositionOnCollider()
    {
        Vector3 randomPointOnUnitSphere = CharacterController.bounds.extents.magnitude * UnityEngine.Random.onUnitSphere;

        return CharacterController.ClosestPointOnBounds(CharacterController.bounds.center + randomPointOnUnitSphere);
    }

    /// <summary>
    /// Applies a launch force to the entity in the specified direction with the given force and stun duration.
    /// Override this function to add custom launch logic.
    /// </summary>
    /// <param name="direction">The direction in which to apply the launch force.</param>
    /// <param name="force">The force of the launch.</param>
    public virtual void Launch(Vector3 direction, float force)
    {
        // Calculate the resulting change in velocity from the impulse
        Vector3 deltaVelocity = (force * direction.normalized) / mass;

        AllowChangeFromGroundedToAirborne();
        IsGrounded = false;

        // Apply the change to the current velocity
        velocity = deltaVelocity;
    }

    /// <summary>
    /// Tries to change the state of the entity to the launch state with the specified direction, force, and stun duration.
    /// If the current state is the death state or already the launch state, it does nothing.
    /// Override this function to modify the blocking states.
    /// </summary>
    /// <param name="direction">The direction in which to apply the launch force.</param>
    /// <param name="force">The force of the launch.</param>
    /// <param name="stunDuration">The duration of the stun caused by the launch.</param>
    public virtual void TryChangeToLaunchState(Vector3 direction, float force, float stunDuration)
    {
        if (CurrentState == EntityDeathState) return;
        if (CurrentState == EntityLaunchState) return;

        EntityLaunchState.SetLaunchSettings(direction, force, stunDuration);
        ChangeState(EntityLaunchState);
    }

    /// <summary>
    /// Forces the state of the entity to the launch state with the specified direction, force, and stun duration.
    /// If the current state is the death state, it does nothing.
    /// Override this function to modify the blocking states.
    /// </summary>
    /// <param name="direction">The direction in which to apply the launch force.</param>
    /// <param name="force">The force of the launch.</param>
    /// <param name="stunDuration">The duration of the stun caused by the launch.</param>
    public virtual void ForceChangeToLaunchState(Vector3 direction, float force, float stunDuration)
    {
        if (CurrentState == EntityDeathState) return;

        EntityLaunchState.SetLaunchSettings(direction, force, stunDuration);
        ChangeState(EntityLaunchState, true);
    }

    /// Retrieves a list of entities within a specified area of effect (AOE) centered at the given hit position.
    /// List is sorted from closest to farthest entity from the hit position.
    /// By default, the list will include dead entities.
    /// </summary>
    /// <param name="hitPosition">The center position of the AOE.</param>
    /// <param name="radius">The radius of the AOE.</param>
    /// <param name="willGetDyingEntities">Whether to get dead entities.</param>
    /// <returns>A list of entities within the AOE, ordered by their distance from the hit position.</returns>
    public static List<Entity> GetEntitiesThroughAOE(Vector3 hitPosition, float radius, bool willGetDyingEntities = true)
    {
        List<Entity> entities = new List<Entity>();

        Collider[] hits = Physics.OverlapSphere(hitPosition, radius, LayerMask.GetMask("Entity"));
        if (hits == null) return entities;
        if (hits.Length == 0) return entities;

        foreach (Collider hit in hits)
        {
            Entity potentialTarget = hit.GetComponent<Entity>();
            if (potentialTarget == null) continue;
            if(!willGetDyingEntities && potentialTarget.CurrentState == potentialTarget.EntityDeathState) continue;

            entities.Add(potentialTarget);
        }

        return entities.OrderBy(target => Vector3.SqrMagnitude(hitPosition - target.transform.position)).ToList();
    }

    /// <summary>
    /// Applies area of effect damage to entities within a given radius.
    /// </summary>
    /// <param name="attacker">The entity causing the damage.</param>
    /// <param name="center">The center position of the AOE.</param>
    /// <param name="radius">The radius within which entities will be damaged.</param>
    /// <param name="damageMultiplier">The multiplier of damage to apply to each entity.</param>
    /// <param name="willTryStagger">Whether to try to stagger the entites hit.</param>
    /// <returns>A list of entities that were damaged.</returns>
    public static List<Entity> DamageEnemyEntitiesWithAOE(Entity attacker, Vector3 center, float radius, float damageMultiplier, bool willTryStagger = true)
    {
        List<Entity> entitiesInRadius = GetEntitiesThroughAOE(center, radius, false);
        List<Entity> entitiesDamaged = new List<Entity>();

        foreach (Entity entityHit in entitiesInRadius)
        {
            if (entityHit.Team == attacker.Team) continue; // skip friendly entities

            attacker.DealDamageToOtherEntity(entityHit,
                attacker.CalculateDamage(damageMultiplier),
                entityHit.CharacterController.ClosestPointOnBounds(center),
                willTryStagger);

            entitiesDamaged.Add(entityHit);
        }

        return entitiesDamaged;
    }

    /// <summary>
    /// Damages entities within a specified area of effect (AOE) and launches them in a given direction with a specified force and stun duration.
    /// </summary>
    /// <param name="attacker">The entity initiating the AOE damage.</param>
    /// /// <param name="center">The center position of the AOE.</param>
    /// <param name="radius">The radius of the AOE.</param>
    /// <param name="damageMultiplier">The multiplier of damage to apply to the entities within the AOE.</param>
    /// <param name="launchForce">The force with which to launch the entities within the AOE.</param>
    /// <param name="stunDuration">The duration of the stun effect applied to the entities within the AOE.</param>
    public static void DamageEnemyEntitiesWithAOELaunch(Entity attacker, Vector3 center, float radius, float damageMultiplier, float launchForce, float stunDuration)
    {
        List<Entity> entitiesHit = DamageEnemyEntitiesWithAOE(attacker, center, radius, damageMultiplier, false);

        foreach (Entity entityHit in entitiesHit)
        {
            Vector3 direction = (entityHit.GetColliderCenterPosition() - center).normalized;

            entityHit.ForceChangeToLaunchState(direction, launchForce, stunDuration);
        }
    }

    /// <summary>
    /// Checks if the given collider belongs to the entity or its child colliders.
    /// </summary>
    /// <param name="hit">The collider to check.</param>
    /// <returns>True if the collider belongs to the Charger or its child colliders, false otherwise.</returns>
    public bool IsOwnCollider(Collider hit)
    {
        Entity selfEntity = hit.GetComponentInParent<Entity>();

        if (selfEntity == null) return false;
        if (selfEntity == this) return true;

        return false;
    }

    /// <summary>
    /// Checks if the enemy hit a wall.
    /// </summary>
    /// <param name="hit">The collider that was hit.</param>
    /// <returns>True if the entity hit a wall, false otherwise.</returns>
    public bool DidHitWall(Collider hit)
    {
        return hit.gameObject.layer == LayerMask.NameToLayer("Ground");
    }

    /// <summary>
    /// Checks if entity hit a friendly entity.
    /// Hit must come from Damageable Colliders layer;
    /// </summary>
    /// <param name="hit">The collider that was hit.</param>
    /// <param name="entity">The friendly entity that was hit.</param>
    /// <returns>True if the entity hit a friendly entity, false otherwise.</returns>
    public bool DidHitFriendlyEntity(Collider hit, out Entity entity)
    {
        entity = hit.GetComponentInParent<Entity>();

        if (entity == null) entity = hit.GetComponent<Entity>();
        if (entity.Team != Team) return false;

        return true;
    }

    /// <summary>
    /// Checks if entity hit an enemy entity.
    /// Hit must come from Damageable Colliders layer;
    /// </summary>
    /// <param name="hit">The collider that was hit.</param>
    /// <param name="entity">The enemy entity that was hit.</param>
    /// <returns>True if the entity hit an enemy entity, false otherwise.</returns>
    public bool DidHitEnemyEntity(Collider hit, out Entity entity)
    {
        entity = hit.GetComponentInParent<Entity>();

        if (entity == null) return false;
        if (entity.Team == Team) return false;

        return true;
    }
}
