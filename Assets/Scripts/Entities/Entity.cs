using DG.Tweening;
using KBCore.Refs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;
using UnityEngine.Pool;
using static UnityEngine.EventSystems.EventTrigger;

public class Entity : MonoBehaviour, IPoolableObject
{
    #region References
    [Header("Entity: References")]
    [SerializeField, Self] private protected CharacterController controller;
    [SerializeField, Self] private protected Animator animator;
    [field: SerializeField, Anywhere] public GlobalPhysicsSettings PhysicsSettings { get; private set; }
    [SerializeField, Anywhere] private protected Transform model;
    private Dictionary<Renderer, Color[]> originalColors = new Dictionary<Renderer, Color[]>();
    #endregion

    #region Health Variables
    [field: Header("Entity: Health")]
    [field: SerializeField] public int CurrentHealth { get; protected set; }
    [field: Tooltip("Max health for entity. Set to 0 for invicibility.")][field: SerializeField] public int MaxHealth { get; protected set; }
    #endregion

    #region Level Variables
    [field: Header("Entity: Level")]
    [field: SerializeField] public int Level { get; protected set; }
    #endregion

    #region Speed Variables
    [Header("Entity: Speed")]
    [SerializeField] private protected float baseSpeed = 3f;
    [SerializeField] private protected float rotationSpeed = 5f;
    [SerializeField] private protected float mass = 1f;
    private protected Vector3 velocity;
    public Vector3 Velocity => velocity;
    public float SpeedModifier { get; protected set; } = 1f;
    public float StatusSpeedModifier { get; protected set; } = 1f;
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
    #endregion

    #region Team Variables
    [field: Header("Entity: Team")]
    [field: SerializeField] public int Team { get; private set; }
    #endregion

    #region Attack Variables
    [field: Header("Entity: Attack")]
    [field: SerializeField] public Vector2Int BaseDamageRange { get; protected set; } = new Vector2Int(10, 15);
    [field: SerializeField] public float DamageModifier { get; protected set; } = 1f;
    [HideInInspector] public bool UseRootMotion;
    #endregion

    #region Stagger Variables
    [field: Header("Entity: Stagger")]
    [field: SerializeField] public float StaggerDuration { get; protected set; } = 0.5f;
    #endregion

    #region Movement Events
    [HideInInspector] public UnityEvent<Vector3> OnGrounded = new UnityEvent<Vector3>(); // 1st arg: where you grounded
    [HideInInspector] public UnityEvent<Vector3> OnAirborne = new UnityEvent<Vector3>(); // 1st arg: where you left ground
    private bool prevIsGrounded;
    #endregion

    #region Combat Events
    [HideInInspector] public UnityEvent<int, Vector3, GameObject> OnEntityTakeDamage = new UnityEvent<int, Vector3, GameObject>(); // passes the hit point and the source of the damage
    [HideInInspector] public UnityEvent<GameObject> OnEntityDeath = new UnityEvent<GameObject>(); // passes the killer gameObject
    [HideInInspector] public UnityEvent<Entity> OnKillEntity = new UnityEvent<Entity>(); // passes the victim entity
    private protected GameObject lastHitSource;
    #endregion

    #region Local Time Scale
    [field: Header("Local Time Scale")]
    [field: SerializeField] public float LocalTimeScale { get; protected set; } = 1f;
    public float LocalDeltaTime => Time.deltaTime * LocalTimeScale;
    #endregion

    #region Pooling Variables
    private ObjectPool<GameObject> pool;
    #endregion

    #region States
    public EntityBaseState CurrentState { get; private set; }
    public EntityBaseState DefaultState { get; private set; }
    public EntityEmptyState EntityEmptyState { get; protected set; }
    public EntityStaggeredState EntityStaggeredState { get; protected set; }
    public EntityDeathState EntityDeathState { get; protected set; }
    public EntityLaunchState EntityLaunchState { get; protected set; }

    /// <summary>
    /// Initializes the states for the entity.
    /// Override this function to add more states to the entity.
    /// Entity states can be initialized as inherited versions of those states.
    /// </summary>
    private protected virtual void InitializeStates()
    {
        //makes new state scripts for the entity to use
        EntityEmptyState = new EntityEmptyState(this);
        EntityDeathState = new EntityDeathState(this);
        EntityLaunchState = new EntityLaunchState(this);
        EntityStaggeredState = new EntityStaggeredState(this);
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
    /// Change the state machine state to the specified new state if the current state is not the same as the new state.
    /// </summary>
    /// <param name="state">The new state to change to.</param>
    public void ChangeState(EntityBaseState state)
    {
        if (CurrentState == EntityDeathState) return;
        if (CurrentState == state) return;

        CurrentState.OnExit();
        CurrentState = state;
        CurrentState.OnEnter();
    }

    /// <summary>
    /// Forces a change of state to the specified new state even when in that same state.
    /// </summary>
    /// <param name="newState">The new state to change to.</param>
    public void ForceChangeState(EntityBaseState newState)
    {
        if (CurrentState == EntityDeathState) return;

        CurrentState.OnExit();
        CurrentState = newState;
        CurrentState.OnEnter();
    }
    #endregion

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
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

        CurrentHealth = MaxHealth;

        SetStartState(EntityEmptyState);
        SetLocalTimeScale(1f);
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

        CacheOriginalTints();
    }

    private void Update()
    {
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
        CurrentState?.Update();

        HandleAnimations();
        EvaluateMovementSpeed();

        HandleGrounded();
        HandleAirborne();

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
        CurrentState?.FixedUpdate();

        CheckGrounded();
        HandleYVelocity();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        CurrentState?.OnControllerColliderHit(hit);
    }

    /// <summary>
    /// Handles the collision enter event for the entity.
    /// Override this function if you want to add custom collision enter logic.
    /// </summary>
    /// <param name="collision">The collision data.</param>
    private protected virtual void OnOnControllerColliderHit(ControllerColliderHit hit)
    {
        CurrentState?.OnControllerColliderHit(hit);
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
        Vector3 desiredAnimationMovement = modelScale * animator.deltaPosition;
        desiredAnimationMovement.y = 0f;

        controller.Move(desiredAnimationMovement);
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

    /// <summary>
    /// Checks if the entity is grounded.
    /// </summary>
    /// <returns>True if the entity is grounded, false otherwise.</returns>
    private protected bool GetIsGrounded()
    {
        return Physics.CheckSphere(transform.position + 9f * controller.radius / 10f * Vector3.up, controller.radius, LayerMask.GetMask("Ground"));
    }

    /// <summary>
    /// Gets the list of RaycastHit objects below the entity within a specified distance and on specified layers.
    /// </summary>
    /// <param name="mask">The layer mask to filter the raycast hits.</param>
    /// <param name="distance">The maximum distance to perform the raycast.</param>
    /// <returns>A list of RaycastHit objects representing the hits below the entity.</returns>
    public List<RaycastHit> GetHitsBelowEntity(LayerMask mask, float distance)
    {
        RaycastHit[] hits = Physics.SphereCastAll(GetColliderCenterPosition(), controller.radius, Vector3.down, distance + Vector3.Distance(GetColliderCenterPosition(), transform.position), mask);
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

            velocity.y = PhysicsSettings.GroundedYVelocity;
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
    private protected virtual void HandleYVelocity()
    {
        if (!IsGrounded)
        {
            velocity.y += LocalDeltaTime * PhysicsSettings.Gravity;
        }
    }

    /// <summary>
    /// Applies gravity to the entity if it is not grounded.
    /// Override this function if you want to add custom gravity logic.
    /// </summary>
    public virtual void ApplyGravity()
    {
        controller.Move(LocalDeltaTime * velocity.y * Vector3.up);
    }

    /// <summary>
    /// Slides off other entities if there are any below the entity.
    /// </summary>
    private void SlideOffOtherEntities()
    {
        if (GetHitsBelowEntity(LayerMask.GetMask("Entity"), 1f).Count > 0)
        {
            ForceUpdateGroundedVelocity(transform.forward, 3f);
            GroundedMove();
        }
    }

    /// <summary>
    /// Resets the Y velocity of the entity to zero.
    /// </summary>
    public void ResetYVelocity()
    {
        velocity.y = 0f;
    }

    /// <summary>
    /// Gets the grounded velocity of the entity.
    /// </summary>
    /// <returns>The grounded velocity.</returns>
    public Vector3 GetGroundedVelocity()
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
    /// Moves the entity while it is grounded.
    /// Override this function if you want to add custom grounded movement logic.
    /// </summary>
    public virtual void GroundedMove()
    {
        controller.Move(GetGroundedVelocity() * LocalDeltaTime);
    }

    /// <summary>
    /// Updates the grounded velocity of the entity based on the given direction.
    /// </summary>
    /// <param name="direction">The direction of movement.</param>
    public void UpdateGroundedVelocity(Vector3 direction)
    {
        Vector3 groundedVelocity = MovementSpeed * new Vector3(direction.x, 0f, direction.z).normalized;

        velocity.x = groundedVelocity.x;
        velocity.z = groundedVelocity.z;
    }

    /// <summary>
    /// Forces an update to the grounded velocity of the entity, ignoring the current speed modifier.
    /// </summary>
    /// <param name="direction">The direction of the velocity.</param>
    /// <param name="speed">The speed of the velocity.</param>
    public void ForceUpdateGroundedVelocity(Vector3 direction, float speed)
    {
        Vector3 groundedVelocity = speed * new Vector3(direction.x, 0f, direction.z).normalized;

        velocity.x = groundedVelocity.x;
        velocity.z = groundedVelocity.z;
    }

    /// <summary>
    /// Handles the animations of the entity.
    /// Sets the MovementSpeed parameter for the FlatMovement blend tree
    /// </summary>
    private protected virtual void HandleAnimations()
    {
        totalSpeedModifierForAnimation = Mathf.Lerp(totalSpeedModifierForAnimation, SpeedModifier, 7.5f * LocalDeltaTime);

        animator.SetFloat("MovementSpeed", totalSpeedModifierForAnimation);
        animator.speed = LocalTimeScale;
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
        if (pool == null)
        {
            Destroy(gameObject);
            return;
        }

        pool.Release(gameObject);
    }

    /// <summary>
    /// Attempts to notify the entity that killed this entity by checking if the last hit source is an entity.
    /// If this fails, then the soruce that killed this entity is not an entity and cannot be notified.
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
    /// Clamps the current health between 0 and MaxHealth. Makes sure the entity cannot have more health than its max health and cannot have negative health.
    /// </summary>
    private void HandleHealth()
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
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

        OnEntityTakeDamage?.Invoke(damage, hitPoint, source);

        if(willTryStagger) TryChangeStaggeredState();

        AttemptToSpawnHitNumbers(damage, hitPoint, Color.red);

        CurrentHealth -= damage;

        lastHitSource = source;

        //after calculating current health, check if the player has taken enough damage to die
        if (CurrentHealth <= 0 && MaxHealth > 0)
        {
            OnDeath();
        }
    }

    /// <summary>
    /// Determines if the entity will die from the given damage.
    /// </summary>
    /// <param name="damage">The amount of damage.</param>
    /// <returns>True if the entity will die, false otherwise.</returns>
    public virtual bool WillDieFromDamage(int damage)
    {
        return MaxHealth > 0 && CurrentHealth - damage <= 0;
    }

    /// <summary>
    /// Tries to change the state of the entity to the staggered state.
    /// If the current state is already the fling state, it does nothing.
    /// </summary>
    private protected virtual void TryChangeStaggeredState()
    {
        if (CurrentState == EntityLaunchState) return;

        ForceChangeState(EntityStaggeredState);
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
        CurrentHealth += health;
        AttemptToSpawnHitNumbers(health, gameObject.transform.position + Vector3.up, Color.green);
    }

    /// <summary>
    /// Kills the entity by dealing maximum damage to itself.
    /// Doesn't work if the entity has max health set to 0.
    /// </summary>
    public void Kill()
    {
        TakeDamage(int.MaxValue, transform.position, gameObject);
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
    /// Changes the team of the entity to the specified new team.
    /// Equal teams cannot damage each other.
    /// </summary>
    /// <param name="newTeam">The new team to assign to the entity.</param>
    public void ChangeTeam(int newTeam)
    {
        Team = newTeam;
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
    /// Transitions the animator to the specified animation using the specified transition duration and layer.
    /// </summary>
    /// <param name="animation">The name of the animation to transition to.</param>
    /// <param name="transitionDuration">The duration of the transition.</param>
    public void TransitionToAnimation(string animation, float transitionDuration = 0.1f, int layer = 0)
    {
        animator.CrossFadeInFixedTime(animation, transitionDuration, layer);
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
        MovementSpeed = StatusSpeedModifier * SpeedModifier * baseSpeed;
    }

    /// <summary>
    /// Rotates the entity to face the specified target position with a speed of rotationSpeed.
    /// Must be called in Update to work.
    /// Returns the target rotation of the entity.
    /// Override this function if you want custom LookAt behavior.
    /// </summary>
    /// <param name="target">The position to look at.</param>
    public virtual Quaternion LookAt(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * LocalDeltaTime);

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
    /// <returns>A list of nearby entities.</returns>
    public List<Entity> GetNearbyHostileEntities(float radius)
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
    /// <returns>A list of nearby entities of the specified type.</returns>
    public List<T> GetNearbyHostileEntitiesByType<T>(float radius) where T : Entity
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

            T potentialTargetT = potentialTarget as T;

            targets.Add(potentialTargetT);
        }

        return targets.OrderBy(target => Vector3.SqrMagnitude(transform.position - target.transform.position)).ToList();
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
        Collider baseCollider = GetComponent<Collider>();
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
        return GetComponent<Collider>().bounds.center;
    }

    /// <summary>
    /// Returns a random position on the collider of the entity.
    /// </summary>
    /// <returns>The random position on the collider.</returns>
    public Vector3 GetRandomPositionOnCollider()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("No collider found on entity.");
            return Vector3.zero;
        }

        Vector3 randomPointOnUnitSphere = collider.bounds.extents.magnitude * Random.onUnitSphere;

        return collider.ClosestPointOnBounds(collider.bounds.center + randomPointOnUnitSphere);
    }

    /// <summary>
    /// Sets the object pool for the entity.
    /// Must be used if the entity is pooled.
    /// </summary>
    /// <param name="objectPool">The object pool to set.</param>
    public void SetObjectPool(ObjectPool<GameObject> objectPool)
    {
        pool = objectPool;
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

        IsGrounded = false;
        inAirTimer = 0.01f;

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
        ForceChangeState(EntityLaunchState);
    }
    
    #region Tinting Functions
    /// <summary>
    /// Caches the original tints of the renderers in the character model.
    /// </summary>
    private void CacheOriginalTints()
    {
        // Get all renderers in the character model, including any child objects
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            if (renderer.TryGetComponent(out Weapon weapon)) continue;

            Color[] colors = new Color[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors[i] = renderer.materials[i].color;
                }
            }
            originalColors.Add(renderer, colors);
        }
    }

    /// <summary>
    /// Tweens the entity's tint color to the specified new color.
    /// </summary>
    /// <param name="newColor">The new color to tween to.</param>
    public void TweenTintEntity(Color newColor)
    {
        foreach (Renderer renderer in originalColors.Keys)
        {
            DOTween.Kill(renderer);
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.DOColor(newColor, 0.2f);
                }
            }
        }
    }

    /// <summary>
    /// Tweens the entity back to its original colors.
    /// </summary>
    public void TweenUnTintEntity()
    {
        foreach (KeyValuePair<Renderer, Color[]> entry in originalColors)
        {
            Renderer renderer = entry.Key;
            Color[] colors = entry.Value;

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                DOTween.Kill(renderer);
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    renderer.materials[i].DOColor(colors[i], 0.2f);
                }
            }
        }
    }

    /// <summary>
    /// Immediately resets the tint of the entity to its original colors.
    /// </summary>
    public void ResetTint()
    {
        foreach (KeyValuePair<Renderer, Color[]> entry in originalColors)
        {
            Renderer renderer = entry.Key;
            Color[] colors = entry.Value;

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                DOTween.Kill(renderer);
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    renderer.materials[i].color = colors[i];
                }
            }
        }
    }
    #endregion

    /// <summary>
    /// Sets the status speed modifier for the entity.
    /// </summary>
    /// <param name="newModifer">The new status speed modifier.</param>
    public void SetStatusSpeedModifier(float newModifer)
    {
        StatusSpeedModifier = newModifer;
    }

    /// <summary>
    /// Sets the damage modifier for the entity.
    /// </summary>
    /// <param name="newModifier">The new damage modifier value.</param>
    public void SetDamageModifier(float newModifier)
    {
        DamageModifier = newModifier;
    }

    /// <summary>
    /// Sets the local time scale for the entity.
    /// Cant set time scale if entity is dead.
    /// </summary>
    /// <param name="newTimeScale">The new time scale value.</param>
    public void SetLocalTimeScale(float newTimeScale)
    {
        if(CurrentState == EntityDeathState) return;

        LocalTimeScale = newTimeScale;
    }

    /// <summary>
    /// Calculates the damage based on the given percentage.
    /// </summary>
    /// <param name="percent">The percentage of the damage range to calculate.</param>
    /// <returns>The calculated damage value.</returns>
    public int CalculateDamage(float percent)
    {
        Vector2Int modifiedDamageRange = Vector2Int.RoundToInt(
            (percent / 100f) * DamageModifier * new Vector2(BaseDamageRange.x, BaseDamageRange.y)
            );

        return Random.Range(modifiedDamageRange.x, modifiedDamageRange.y);
    }
    
    /// Retrieves a list of entities within a specified area of effect (AOE) centered at the given hit position.
    /// List is sorted from closest to farthest entity from the hit position.
    /// </summary>
    /// <param name="hitPosition">The center position of the AOE.</param>
    /// <param name="radius">The radius of the AOE.</param>
    /// <returns>A list of entities within the AOE, ordered by their distance from the hit position.</returns>
    public static List<Entity> GetEntitiesThroughAOE(Vector3 hitPosition, float radius)
    {
        List<Entity> entities = new List<Entity>();

        Collider[] hits = Physics.OverlapSphere(hitPosition, radius, LayerMask.GetMask("Entity"));
        if (hits == null) return entities;
        if (hits.Length == 0) return entities;

        foreach (Collider hit in hits)
        {
            Entity potentialTarget = hit.GetComponent<Entity>();
            if (potentialTarget == null) continue;

            entities.Add(potentialTarget);
        }

        return entities.OrderBy(target => Vector3.SqrMagnitude(hitPosition - target.transform.position)).ToList();
    }
}
