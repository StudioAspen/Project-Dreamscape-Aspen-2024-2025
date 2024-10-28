using KBCore.Refs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class Entity : MonoBehaviour, IPoolableObject
{
    [Header("Entity: References")]
    [SerializeField, Self] private protected Animator animator;
    [SerializeField] private protected GlobalPhysicsSettings physicsSettings;

    [field: Header("Entity: Settings")]
    [field: SerializeField] public int CurrentHealth { get; protected set; }
    [field: SerializeField] public int MaxHealth { get; protected set; }
    [field: SerializeField] public int Level { get; protected set; }

    [HideInInspector] public bool IsGrounded;
    protected private float inAirTimer;
    protected private bool fallVelocityApplied;

    public float SpeedModifier { get; protected set; } = 1f;
    [SerializeField] protected private float baseSpeed = 3f;
    [SerializeField] protected private Vector3 velocity;
    [SerializeField] protected private float rotationSpeed = 5f;

    [SerializeField] private protected float targetDetectionRadius = 10f;

    public int Team { get; private set; }

    protected private GameObject lastHitSource;
    [HideInInspector] public UnityEvent<Vector3, GameObject> OnEntityTakeDamage = new UnityEvent<Vector3, GameObject>();
    [HideInInspector] public UnityEvent<GameObject> OnEntityDeath = new UnityEvent<GameObject>();
    [HideInInspector] public UnityEvent<Entity> OnKillEntity = new UnityEvent<Entity>();

    #region States
    public BaseState CurrentState { get; private set; }
    public BaseState DefaultState { get; private set; }
    public EntityEmptyState EntityEmptyState { get; protected set; }
    public EntityHitState EntityHitState { get; protected set; }
    public EntityDeathState EntityDeathState { get; protected set; }
    #endregion

    private ObjectPool<GameObject> pool;

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

    protected virtual void OnAwake()
    {
        InitializeStates();
    }

    private void OnEnable()
    {
        OnOnEnable();
    }

    protected virtual void OnOnEnable()
    {
        CurrentHealth = MaxHealth;

        SetStartState(EntityEmptyState);
    }

    private void OnDisable()
    {
        OnOnDisable();
    }

    protected virtual void OnOnDisable()
    {

    }

    private void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        SetDefaultState(EntityEmptyState);

        IgnoreMyOwnColliders();
    }

    private void Update()
    {
        OnUpdate();   
    }

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

    protected virtual void OnFixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }

    protected virtual void InitializeStates()
    {
        //makes new state scripts for the entity to use
        EntityEmptyState = new EntityEmptyState(this);
        EntityHitState = new EntityHitState(this);
        EntityDeathState = new EntityDeathState(this);
    }

    protected void SetStartState(BaseState state)
    {
        CurrentState = state;
        CurrentState.OnEnter();
    }

    protected void SetDefaultState(BaseState state)
    {
        DefaultState = state;
    }

    public void ChangeState(BaseState state)
    {
        if (CurrentState == EntityDeathState) return;
        if (CurrentState == state) return;
        //if (CurrentState.GetType() == state.GetType()) return;

        CurrentState.OnExit();
        CurrentState = state;
        CurrentState.OnEnter();
    }

    protected virtual void CheckGrounded() { }

    protected virtual void OnDeath()
    {
        OnEntityDeath?.Invoke(lastHitSource);

        AttemptToNotifyKiller();

        ChangeState(EntityDeathState);
    }

    public virtual void Die()
    {
        if(pool == null)
        {
            Destroy(gameObject);
            return;
        }

        pool.Release(gameObject);
    }

    protected virtual void AttemptToNotifyKiller()
    {
        Entity killer = lastHitSource.GetComponent<Entity>();
        if (killer == null) return;

        killer.OnKill(this);
    }

    private void HandleHealth()
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }

    public virtual void TakeDamage(int dmg, Vector3 hitPoint, GameObject source)
    {
        if (CurrentState == EntityDeathState) return;

        ChangeState(DefaultState);
        ChangeState(EntityHitState);

        AttemptToSpawnHitNumbers(dmg, hitPoint);

        CurrentHealth -= dmg;

        lastHitSource = source;

        OnEntityTakeDamage?.Invoke(hitPoint, source);

        //after calculating current health, check if the player has taken enough damage to die
        if(CurrentHealth <= 0 && MaxHealth > 0)
        {
            OnDeath();
        }
    }

    private protected void AttemptToSpawnHitNumbers(int dmg, Vector3 hitPoint)
    {
        ObjectPooler spawner = GameObject.Find("HitNumberPooler").GetComponent<ObjectPooler>();
        if (spawner == null) return;

        HitNumbers hitNumber = spawner.SpawnObject<HitNumbers>();
        hitNumber.ActivateHitNumberText(dmg, hitPoint);
    }

    public void Heal(int health)
    {
        CurrentHealth += health;
    }

    public void Kill()
    {
        TakeDamage(int.MaxValue, transform.position, gameObject);
    }

    public void ChangeTeam(int newTeam)
    {
        Team = newTeam;
    }

    public void SetSpeedModifier(float speed)
    {
        SpeedModifier = speed;
    }

    public void DefaultTransitionToAnimation(string animation)
    {
        animator.CrossFadeInFixedTime(animation, 0.1f);
    }

    public void DefaultTransitionToAnimation(string animation, string layer)
    {
        animator.CrossFadeInFixedTime(animation, 0.1f, animator.GetLayerIndex(layer));
    }

    public void TransitionToAnimation(string animation, float transitionDuration)
    {
        animator.CrossFadeInFixedTime(animation, transitionDuration);
    }

    public void TransitionToAnimation(string animation, float transitionDuration, string layer)
    {
        animator.CrossFadeInFixedTime(animation, transitionDuration, animator.GetLayerIndex(layer));
    }

    public virtual void OnKill(Entity entity)
    {
        OnKillEntity?.Invoke(entity);
    }

    public virtual void LookAt(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public float Distance(Vector3 target)
    {
        return Vector3.Distance(target, transform.position);
    }

    public float Distance(Entity entity)
    {
        return Vector3.Distance(entity.transform.position, transform.position);
    }

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
            if(potentialTarget.GetType() != typeof(T)) continue;
            if (potentialTarget.Team == Team) continue;

            T potentialTargetT = potentialTarget as T;

            targets.Add(potentialTargetT);
        }

        return targets.OrderBy(target => Vector3.SqrMagnitude(transform.position - target.transform.position)).ToList();
    }

    public bool IsBlockedFromEntity(Entity entity)
    {
        LayerMask ignoreLayers = ~LayerMask.GetMask("Entity", "Damageable Entity", "Damage Collider", "SelectionSphere");

        RaycastHit hit;
        Physics.Raycast(transform.position, entity.transform.position - transform.position, out hit, Distance(entity), ignoreLayers);

        if (hit.collider == null) return false;

        return true;
    }

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

    public void SetObjectPool(ObjectPool<GameObject> objectPool)
    {
        pool = objectPool;
    }
}
