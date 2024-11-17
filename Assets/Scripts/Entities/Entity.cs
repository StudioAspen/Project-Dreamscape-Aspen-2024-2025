using KBCore.Refs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class Entity : MonoBehaviour, IPoolableObject
{
    #region References Variables
    [Header("Entity: References")]
    [SerializeField, Self] private protected Animator animator;
    [SerializeField, Anywhere] private protected GlobalPhysicsSettings physicsSettings;
    [SerializeField, Anywhere] private protected Transform model;
    #endregion

    #region Health Variables
    [field: Header("Entity: Health")]
    [field: SerializeField] public int CurrentHealth { get; protected set; }
    [field: Tooltip("Max health for entity. Set to 0 for invicibility.")] [field: SerializeField] public int MaxHealth { get; protected set; }
    #endregion

    #region Level Variables
    [field: Header("Entity: Level")]
    [field: SerializeField] public int Level { get; protected set; }
    #endregion

    #region Speed Variables
    [Header("Entity: Speed")]
    [SerializeField] protected private float baseSpeed = 3f;
    [SerializeField] protected private float rotationSpeed = 5f;
    public float SpeedModifier { get; protected set; } = 1f;
    protected private Vector3 velocity;
    #endregion

    #region Airborne Variables
    [HideInInspector] public bool IsGrounded;
    protected private float inAirTimer;
    protected private bool fallVelocityApplied;
    #endregion

    private List<StatusEffectSO> statusEffectors = new List<StatusEffectSO>();
    public enum EntityStats
    {
        MAXHEALTH,
        SPEED,
        ATTACKPOWER
    }

    #region Target Detection Variables
    [Header("Entity: Target Detection")]
    [SerializeField] private protected float targetDetectionRadius = 10f;
    #endregion

    #region Team Variables
    public int Team { get; private set; }
    #endregion

    #region Combat Events
    [HideInInspector] public UnityEvent<Vector3, GameObject> OnEntityTakeDamage = new UnityEvent<Vector3, GameObject>();
    [HideInInspector] public UnityEvent<GameObject> OnEntityDeath = new UnityEvent<GameObject>();
    [HideInInspector] public UnityEvent<Entity> OnKillEntity = new UnityEvent<Entity>();
    protected private GameObject lastHitSource;
    #endregion

    #region Stagger Variables
    [field: Header("Entity: Stagger")]
    [field: SerializeField] public float StaggerDuration { get; protected set; } = 0.5f;
    #endregion

    #region Pooling Variables
    private ObjectPool<GameObject> pool;
    #endregion

    #region States
    public BaseState CurrentState { get; private set; }
    public BaseState DefaultState { get; private set; }
    public EntityEmptyState EntityEmptyState { get; protected set; }
    public EntityStaggeredState EntityStaggeredState { get; protected set; }
    public EntityDeathState EntityDeathState { get; protected set; }
    public EntityFlingState EntityFlingState { get; protected set; }

    /// <summary>
    /// Initializes the states for the entity.
    /// Override this function to add more states to the entity.
    /// Entity states can be initialized as inherited versions of those states.
    /// </summary>
    protected virtual void InitializeStates()
    {
        //makes new state scripts for the entity to use
        EntityEmptyState = new EntityEmptyState(this);
        EntityDeathState = new EntityDeathState(this);
        EntityFlingState = new EntityFlingState(this);
        EntityStaggeredState = new EntityStaggeredState(this);
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
    protected virtual void OnAwake()
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
    protected virtual void OnOnEnable()
    {
        CurrentHealth = MaxHealth;

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
    protected virtual void OnOnDisable()
    {

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
    protected virtual void OnStart()
    {
        SetDefaultState(EntityEmptyState);

        IgnoreMyOwnColliders();
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
    protected virtual void OnUpdate()
    {
        //if CurrentState isn't null, run it's Update function
        //the states are regular C# scripts because if we did another Monobehavior, it'd add a second call to Update which isn't really necessary n takes extra resources..
        CurrentState?.Update();

        CheckGrounded();
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
    protected virtual void OnFixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }

    /// <summary>
    /// Sets the start state of the entity.
    /// </summary>
    /// <param name="state">The start state to set.</param>
    protected void SetStartState(BaseState state)
    {
        CurrentState = state;
        CurrentState.OnEnter();
    }

    /// <summary>
    /// Sets the default state of the entity.
    /// </summary>
    /// <param name="state">The default state to set.</param>
    protected void SetDefaultState(BaseState state)
    {
        DefaultState = state;
    }

    /// <summary>
    /// Change the state machine state to the specified new state if the current state is not the same as the new state.
    /// </summary>
    /// <param name="state">The new state to change to.</param>
    public void ChangeState(BaseState state)
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
    public void ForceChangeState(BaseState newState)
    {
        if (CurrentState == EntityDeathState) return;

        CurrentState.OnExit();
        CurrentState = newState;
        CurrentState.OnEnter();
    }

    /// <summary>
    /// Handles the IsGrounded bool for the entity. Override this method to add custom grounded checks.
    /// </summary>
    protected virtual void CheckGrounded() { }

    /// <summary>
    /// Handles the death logic for the entity by invoking the OnEntityDeath event and attempting to notify the killer.
    /// Also changes the state to the death state.
    /// Override this function if you want to add custom death logic.
    /// </summary>
    protected virtual void OnDeath()
    {
        OnEntityDeath?.Invoke(lastHitSource);

        AttemptToNotifyKiller();

        ChangeState(EntityDeathState);
    }

    /// <summary>
    /// Fired by the entity's death animation event once the death animation has finished playing.
    /// Releases the entity back to the object pool or destroys it if there is no object pool.
    /// Override this function if you want to add custom logic for how the entity is destroyed.
    /// </summary>
    public virtual void Die()
    {
        if(pool == null)
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
    protected virtual void AttemptToNotifyKiller()
    {
        if (lastHitSource == null) return;

        if(lastHitSource.TryGetComponent(out Entity killer))
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
    /// Also, attempts to spawn hit numbers at the hit point and invokes the OnEntityTakeDamage event.
    /// Entity will be invicible if max health is set to 0.
    /// Override this function if you want to add custom damage taking logic.
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    /// <param name="hitPoint">The point where the entity was hit.</param>
    /// <param name="source">The source of the damage.</param>
    public virtual void TakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        if (CurrentState == EntityDeathState) return;

        ForceChangeState(EntityStaggeredState);

        AttemptToSpawnHitNumbers(damage, hitPoint);

        CurrentHealth -= damage;

        lastHitSource = source;

        OnEntityTakeDamage?.Invoke(hitPoint, source);

        //after calculating current health, check if the player has taken enough damage to die
        if (CurrentHealth <= 0 && MaxHealth > 0)
        {
            OnDeath();
        }
    }

    /// <summary>
    /// Entity doesn't become staggered on hit and cannot take damage if it is in the death state.
    /// Takes damage and updates the entity's health while checking to see if the entity's health reaches below zero.
    /// Also, attempts to spawn hit numbers at the hit point and invokes the OnEntityTakeDamage event.
    /// Entity will be invicible if max health is set to 0.
    /// Override this function if you want to add custom damage taking logic.
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    /// <param name="hitPoint">The point where the entity was hit.</param>
    /// <param name="source">The source of the damage.</param>
    public virtual void TakeDamageWithoutState(int dmg, Vector3 hitPoint, GameObject source)
    {
        if (CurrentState == EntityDeathState) return;

        AttemptToSpawnHitNumbers(dmg, hitPoint);

        CurrentHealth -= dmg;

        lastHitSource = source;

        OnEntityTakeDamage?.Invoke(hitPoint, source);

        //after calculating current health, check if the player has taken enough damage to die
        if (CurrentHealth <= 0 && MaxHealth > 0)
        {
            OnDeath();
        }
    }

    /// <summary>
    /// Attempts to spawn hit numbers at the hit point with the specified damage.
    /// Fails if the HitNumberPooler is not found.
    /// </summary>
    /// <param name="dmg">The amount of damage to display.</param>
    /// <param name="hitPoint">The point where the entity was hit.</param>
    private protected void AttemptToSpawnHitNumbers(int dmg, Vector3 hitPoint)
    {
        ObjectPooler spawner = GameObject.Find("HitNumberPooler").GetComponent<ObjectPooler>();
        if (spawner == null) return;

        HitNumbers hitNumber = spawner.SpawnObject<HitNumbers>();
        hitNumber.ActivateHitNumberText(dmg, hitPoint);
    }

    /// <summary>
    /// Increases the current health of the entity by the specified amount.
    /// </summary>
    /// <param name="health">The amount of health to add.</param>
    public void Heal(int health)
    {
        CurrentHealth += health;
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
    /// Transitions the animator to the specified animation using default transition duration.
    /// </summary>
    /// <param name="animation">The name of the animation to transition to.</param>
    public void DefaultTransitionToAnimation(string animation)
    {
        animator.CrossFadeInFixedTime(animation, 0.1f);
    }

    /// <summary>
    /// Transitions the animator to the specified animation using default transition duration.
    /// Also specifies the layer to transition to.
    /// </summary>
    /// <param name="animation">The name of the animation to transition to.</param>
    public void DefaultTransitionToAnimation(string animation, string layer)
    {
        animator.CrossFadeInFixedTime(animation, 0.1f, animator.GetLayerIndex(layer));
    }

    /// <summary>
    /// Transitions the animator to the specified animation using the specified transition duration.
    /// </summary>
    /// <param name="animation">The name of the animation to transition to.</param>
    /// <param name="transitionDuration">The duration of the transition.</param>
    public void TransitionToAnimation(string animation, float transitionDuration)
    {
        animator.CrossFadeInFixedTime(animation, transitionDuration);
    }

    /// <summary>
    /// Transitions the animator to the specified animation using the specified transition duration and layer.
    /// </summary>
    /// <param name="animation">The name of the animation to transition to.</param>
    /// <param name="transitionDuration">The duration of the transition.</param>
    /// <param name="layer">The layer to transition to.</param>
    public void TransitionToAnimation(string animation, float transitionDuration, string layer)
    {
        animator.CrossFadeInFixedTime(animation, transitionDuration, animator.GetLayerIndex(layer));
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
    /// Rotates the entity to face the specified target position with a speed of rotationSpeed.
    /// Must be called in Update to work.
    /// Override this function if you want custom LookAt behavior.
    /// </summary>
    /// <param name="target">The position to look at.</param>
    public virtual void LookAt(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
    /// Gets a list of nearby entities within the specified radius.
    /// The entities must be on the "Entity" layer and not on the same team as the entity.
    /// The list is sorted from closest to farthest.
    /// </summary>
    /// <param name="radius">The radius within which to search for nearby entities.</param>
    /// <returns>A list of nearby entities.</returns>
    public List<Entity> GetNearbyEntities(float radius)
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
    /// Gets a list of nearby entities of a specific type within the specified radius.
    /// The entities must be on the "Entity" layer and not on the same team as the entity.
    /// The list is sorted from closest to farthest.
    /// </summary>
    /// <typeparam name="T">The type of entities to retrieve.</typeparam>
    /// <param name="radius">The radius within which to search for nearby entities.</param>
    /// <returns>A list of nearby entities of the specified type.</returns>
    public List<T> GetNearbyEntitiesByType<T>(float radius) where T : Entity
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
    /// Sets the object pool for the entity.
    /// Must be used if the entity is pooled.
    /// </summary>
    /// <param name="objectPool">The object pool to set.</param>
    public void SetObjectPool(ObjectPool<GameObject> objectPool)
    {
        pool = objectPool;
    }

    /// <summary>
    /// Gets a random damage value within the specified damage range.
    /// </summary>
    /// <param name="damageRange">The range of damage values.</param>
    /// <returns>A random damage value within the range.</returns>
    public int GetRandomDamageFromRange(Vector2Int damageRange)
    {
        return Random.Range(damageRange.x, damageRange.y);
    }

    /// <summary>
    /// Applies a fling force to the entity in the specified direction with the given force and stun duration.
    /// Override this function to add custom fling logic.
    /// </summary>
    /// <param name="direction">The direction in which to apply the fling force.</param>
    /// <param name="force">The force of the fling.</param>
    /// <param name="stunDuration">The duration of the stun caused by the fling.</param>
    public virtual void Fling(Vector3 direction, float force, float stunDuration)
    {

    }
}
