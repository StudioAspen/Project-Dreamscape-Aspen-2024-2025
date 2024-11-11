using UnityEngine;

public class Charger : Enemy
{
    [field: Header("Charger: Wander Settings")]
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(3f, 5f);

    [field: Header("Charger: Player Detected Settings")]
    [field: SerializeField] public float TargetDetectedDuration { get; private set; } = 2f;
    [field: SerializeField] public float NearbyAttackRadiusThreshold { get; private set; } = 6f;

    [field: Header("Charger: Far Attack Settings")]
    [field: SerializeField] public float FarAttackDuration { get; private set; } = 20f;
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




    [field: Header("Charger: Wind Down Settings")]
    [field: SerializeField] public float WindDownDuration;
    [field: SerializeField] public float DamagedStateDuration;


    [field: Header("Charger: Dazed Settings")]
    [field: SerializeField] public float DazedDuration { get; private set; } = 5f;
    [field: SerializeField] public bool IsDazed;

    #region States
    public ChargerDazedState ChargerDazedState { get; private set; }
    public ChargerWanderState ChargerWanderState { get; private set; }
    public ChargerFarAttackState ChargerFarAttackState { get; private set; }
    public ChargerWindDownState ChargerWindDownState { get; private set; }
    public ChargerTargetDetectedState ChargerTargetDetectedState { get; private set; }
    public ChargerDamagedState ChargerDamagedState { get; private set; }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        EnemyIdleState = new ChargerIdleState(this);
        EnemyChaseState = new ChargerChaseState(this);
        ChargerDazedState = new ChargerDazedState(this);
        ChargerWanderState = new ChargerWanderState(this);
        ChargerFarAttackState = new ChargerFarAttackState(this);
        ChargerWindDownState = new ChargerWindDownState(this);
        ChargerTargetDetectedState = new ChargerTargetDetectedState(this);
        ChargerDamagedState = new ChargerDamagedState(this);
    }
    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();
        SetStartState(ChargerWanderState);
    }

    protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    protected override void OnOnAnimatorMove()
    {
        base.OnOnAnimatorMove();
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public override void TakeDamage(int dmg, Vector3 hitPoint, GameObject source)
    {
        if (CurrentState == EntityDeathState) return;

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
