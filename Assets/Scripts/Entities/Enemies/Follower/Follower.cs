using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Enemy
{
    [field: Header("Follower: Attack Settings")]
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;
    [field: SerializeField] public Vector2Int AttackDamageRange { get; private set; } = new Vector2Int(10, 15);
    [field: SerializeField] public float AttackReadyDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float AttackRecoverDuration { get; private set; } = 1f;


    [field: Header("Follower: Circle Settings")]
    [field: SerializeField] public int CircleFollowerCountThreshold { get; private set; } = 2;
    [field: SerializeField] public float ChangeDirectionInterval { get; private set; } = 0.5f;
    [field: SerializeField] public int ChangeDirectionReciprocal { get; private set; } = 50;
    [field: SerializeField] public float CircleRadius { get; private set; } = 5f;
    [field: SerializeField] public float MaxCircleRadius { get; private set; } = 8f;

    #region States
    public FollowerAttackState FollowerAttackState { get; private set; }
    public FollowerReadyAttackState FollowerReadyAttackState { get; private set; }
    public FollowerAttackRecoverState FollowerAttackRecoverState { get; private set; }
    public FollowerCircleState FollowerCircleState { get; private set; }
    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();

        // SetStartState(EnemyIdleState);
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

    protected override void InitializeStates()
    {
        base.InitializeStates();

        EnemyChaseState = new FollowerChaseState(this);
        FollowerCircleState = new FollowerCircleState(this);
        FollowerAttackState = new FollowerAttackState(this);
        FollowerReadyAttackState = new FollowerReadyAttackState(this);
        FollowerAttackRecoverState = new FollowerAttackRecoverState(this);
    }
}
