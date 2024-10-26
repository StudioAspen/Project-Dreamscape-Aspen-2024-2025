using UnityEngine;

public class Charger : Enemy
{
    [field: Header("Charger: Attack Settings")]
    [field: SerializeField] public float ChargingProcRadius { get; private set; } = 11f;
    [field: SerializeField] public float ChargeSpeed { get; private set; } = 8f;
    [field: SerializeField] public float ChargeDuration { get; private set; } = 3f;
    [field: SerializeField] public float SlowDownDuration { get; private set; } = 2f;
    [field: SerializeField] public float HitboxRadius { get; private set; } = 3f;
    [field: SerializeField] public float RotationSpeed { get; private set; } = 5f;


    #region States
    public ChargerFarAttackState ChargerFarAttackState { get; private set; }
    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();

        SetStartState(ChargerFarAttackState);
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

        SetDefaultState(EnemyIdleState);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        ChargerFarAttackState = new ChargerFarAttackState(this);
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
