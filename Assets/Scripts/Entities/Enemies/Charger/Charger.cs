using UnityEngine;
public class Charger : Enemy
{   
    [field: Header("Charger: Wind Down and Damaged Settings")]
    [field: SerializeField] public float WindDownDuration;
    [field: SerializeField] public float DamagedStateDuration;
    [field: SerializeField] public bool IsDazed;


    [field: Header("Charger: Close Attack Settings")]
    [field: SerializeField] public bool IsInterrupted;
    
    
    
    [field: Header("Charger: Movement Setttings")]
    [field: SerializeField] public int CircleChargerCountThreshold {get; private set;} = 2;
    [field: SerializeField] public float ChangeDirectionInterval {get; private set;} = 0.5f;
    [field: SerializeField] public int ChangeDirectionReciprocal {get; private set;} = 50;
    [field: SerializeField] public float CircleRadius {get; private set;} = 5f;
    [field: SerializeField] public float MaxCircleRadius {get; private set;} = 0f;

    // Charger Attack Far from Joshua
    [field: Header("Charger: Attack Settings")]
    [field: SerializeField] public float ChargingProcRadius { get; private set; } = 11f;
    [field: SerializeField] public float ChargeSpeed { get; private set; } = 8f;
    [field: SerializeField] public float ChargeDuration { get; private set; } = 5f;
    [field: SerializeField] public float SlowDownDuration { get; private set; } = 2f;
    [field: SerializeField] public float HitboxRadius { get; private set; } = 3f;
    [field: SerializeField] public float RotationSpeed { get; private set; } = 5f;
    [field: SerializeField] public float KnockbackForce { get; private set; } = 2f;


    // Enemy Idle and Dazed States from John
    [field: Header("Follower: Attack Settings")]
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;
    [field: SerializeField] public Vector2Int AttackDamageRange { get; private set; } = new Vector2Int(10, 15);
    

    [field: Header("Charger: Idle Settings")]
   
    [field: SerializeField] public float IdleWanderWaitMin { get; private set; } = 1f;
    [field: SerializeField] public float IdleWanderWaitMax { get; private set; } = 3f;


    [field: Header("Charger: Wander Settings")]
    [field: SerializeField] public float WanderMaxRadius { get; private set; } = 5f;
    [field: SerializeField] public float WanderMinRadius { get; private set; } = 1.5f;


   
    [field: Header("Charger: Advanced Wander Settings")]
    [field: SerializeField] public float WanderRaycastDownMaxDistance = 15f;
    [field: SerializeField] public float WanderEndDistanceThreshold = 1.25f;
    [field: SerializeField] public float WanderTimeout = 10f;
    [field: SerializeField] public float WanderNavMeshSampleRadius = .1f;


    [field: Header("Charger: Dazed Settings")]
    [field: SerializeField] public float DazedDuration { get; private set; } = 5f;

    #region States
    public ChargerIdleState ChargerIdleState { get; private set; }
    public ChargerDazedState ChargerDazedState { get; private set; }
    public ChargerFarAttackState ChargerFarAttackState { get; private set; }
    public ChargerWanderState ChargerWanderState { get; private set; }


    protected override void InitializeStates()
    {
        base.InitializeStates();

        ChargerIdleState = new ChargerIdleState(this);
        ChargerDazedState = new ChargerDazedState(this);
        ChargerFarAttackState = new ChargerFarAttackState(this);
        ChargerWanderState = new ChargerWanderState(this);
    }
    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    protected override void OnOnAnimatorMove()
    {
        base.OnOnAnimatorMove();
    }

    protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();
        SetStartState(ChargerIdleState);
    
    }

    protected override void OnStart()
    {
        base.OnStart();
        
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void TakeDamage(int dmg, Vector3 hitPoint, GameObject source)
    {
        if(CurrentState == EntityDeathState) return;

        if (CurrentState != ChargerFarAttackState)
        {
            ChangeState(DefaultState);
            ChangeState(EntityHitState);
        }


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
}
