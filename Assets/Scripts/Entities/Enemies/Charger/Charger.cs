using UnityEngine;

public class Charger : Enemy
{
    [field: Header("Charger: Movement Setttings")]
    [field: SerializeField] public int CircleChargerCountThreshold { get; private set; } = 2;
    [field: SerializeField] public float ChangeDirectionInterval { get; private set; } = 0.5f;
    [field: SerializeField] public int ChangeDirectionReciprocal { get; private set; } = 50;
    [field: SerializeField] public float CircleRadius { get; private set; } = 5f;
    [field: SerializeField] public float MaxCircleRadius { get; private set; } = 0f;


    [field: Header("Charger: Player Detected Settings")]
    [field: SerializeField] public float PlayerDetectedDuration { get; private set; } = 2f;


    [field: Header("Charger: Attack Settings")]
    [field: SerializeField] public float ChargingProcRadius { get; private set; } = 11f;
    [field: SerializeField] public float ChargeSpeedMin { get; private set; } = 8f;
    [field: SerializeField] public float ChargeSpeedMax { get; private set; } = 16f;
    [field: SerializeField] public float ChargeAccelerationInterval { get; private set; } = .05f;
    [field: SerializeField] public float HitboxRadius { get; private set; } = 3f;
    [field: SerializeField] public float TargetFocusTime { get; private set; } = 2f;
    [field: SerializeField] public float KnockbackForce { get; private set; } = 2f;
    [field: SerializeField] public float MaxChargeDuration { get; private set; } = 4f;
    [field: SerializeField] public float MaxChargeDistance { get; private set; } = 25f;

    [field: Header("Charger: Close Attack Settings")]
    [field: SerializeField] public bool IsInterrupted;


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


    [field: Header("Charger: Wind Down Settings")]
    [field: SerializeField] public float WindDownDuration;
    [field: SerializeField] public float DamagedStateDuration;


    [field: Header("Charger: Dazed Settings")]
    [field: SerializeField] public float DazedDuration { get; private set; } = 5f;
    [field: SerializeField] public bool IsDazed;

    #region States
    public ChargerIdleState ChargerIdleState { get; private set; }
    public ChargerDazedState ChargerDazedState { get; private set; }
    public ChargerWanderState ChargerWanderState { get; private set; }
    public ChargerFarAttackState ChargerFarAttackState { get; private set; }
    public ChargerWindDownState ChargerWindDownState { get; private set; }
    public ChargerChaseState ChargerChaseState { get; private set; }
    public ChargerPlayerDetectedState ChargerPlayerDetectedState { get; private set; }
    public ChargerDamagedState ChargerDamagedState { get; private set; }
    

    protected override void InitializeStates()
    {
        base.InitializeStates();

        ChargerIdleState = new ChargerIdleState(this);
        ChargerDazedState = new ChargerDazedState(this);
        ChargerWanderState = new ChargerWanderState(this);
        ChargerFarAttackState = new ChargerFarAttackState(this);
        ChargerWindDownState = new ChargerWindDownState(this);
        ChargerChaseState = new ChargerChaseState(this);
        ChargerPlayerDetectedState = new ChargerPlayerDetectedState(this);
        ChargerDamagedState = new ChargerDamagedState(this);
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
        //base.OnOnEnable();
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


    public override void TakeDamage(int dmg, Vector3 hitPoint, GameObject source) {
        if (CurrentState == EntityDeathState) return;

        if (CurrentState != ChargerFarAttackState) {
            ChangeState(DefaultState);
            ChangeState(EntityHitState);
        }

        AttemptToSpawnHitNumbers(dmg, hitPoint);
        CurrentHealth -= dmg;
        lastHitSource = source;
        OnEntityTakeDamage?.Invoke(hitPoint, source);
        //after calculating current health, check if the player has taken enough damage to die
        if (CurrentHealth <= 0 && MaxHealth > 0) {
            OnDeath();
        }
    }

}
