using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Enemy
{
    [field: Header("Follower: Wander Settings")]
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(3f, 5f);

    [field: Header("Follower: Circle Settings")]
    [field: SerializeField] public int CircleFollowerCountThreshold { get; private set; } = 2;
    [field: SerializeField] public float ChangeDirectionInterval { get; private set; } = 0.5f;
    [field: SerializeField] public int ChangeDirectionReciprocal { get; private set; } = 50;
    [field: SerializeField] public float CircleRadius { get; private set; } = 5f;
    [field: SerializeField] public float MaxCircleRadius { get; private set; } = 8f;

    [field: Header("Follower: Attack Settings")]
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;
    [field: SerializeField] public float AttackPercentDamage { get; private set; } = 100f;
    [field: SerializeField] public float AttackReadyDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float AttackRecoverDuration { get; private set; } = 1f;
    public Weapon Weapon { get; protected set; }

    #region States
    public FollowerAttackStateSO FollowerAttackState { get; private set; }
    public FollowerWanderStateSO FollowerWanderState { get; private set; }
    public FollowerReadyAttackStateSO FollowerReadyAttackState { get; private set; }
    public FollowerAttackRecoverStateSO FollowerAttackRecoverState { get; private set; }
    public FollowerCircleStateSO FollowerCircleState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        EnemyChaseState = EntityBaseStateSO.CreateRuntimeInstance<FollowerChaseStateSO>(this);
        FollowerWanderState = EntityBaseStateSO.CreateRuntimeInstance<FollowerWanderStateSO>(this);
        FollowerCircleState = EntityBaseStateSO.CreateRuntimeInstance<FollowerCircleStateSO>(this);
        FollowerAttackState = EntityBaseStateSO.CreateRuntimeInstance<FollowerAttackStateSO>(this);
        FollowerReadyAttackState = EntityBaseStateSO.CreateRuntimeInstance<FollowerReadyAttackStateSO>(this);
        FollowerAttackRecoverState = EntityBaseStateSO.CreateRuntimeInstance<FollowerAttackRecoverStateSO>(this);
    }
    #endregion

    private protected override void OnAwake()
    {
        base.OnAwake();

        Weapon = GetComponentInChildren<Weapon>();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

        SetStartState(FollowerWanderState);

        FinishAnimation();
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    private protected override void OnStart()
    {
        base.OnStart();

        SetDefaultState(FollowerWanderState);

        FinishAnimation();
    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    private protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public void FinishAnimation()
    {
        IsAttackAnimationPlaying = false;
        DisableWeaponTriggers();
    }

    public void EnableWeaponTriggers()
    {
        Weapon.EnableTriggers();
    }

    public void DisableWeaponTriggers()
    {
        Weapon.DisableTriggers();
    }
}
