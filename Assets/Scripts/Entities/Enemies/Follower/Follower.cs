using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Enemy
{
    #region States
    [field: Header("Follower: States")]
    [field: SerializeField] public FollowerChaseState FollowerChaseState { get; private set; }
    [field: SerializeField] public FollowerAttackState FollowerAttackState { get; private set; }
    [field: SerializeField] public FollowerWanderState FollowerWanderState { get; private set; }
    [field: SerializeField] public FollowerReadyAttackState FollowerReadyAttackState { get; private set; }
    [field: SerializeField] public FollowerAttackRecoverState FollowerAttackRecoverState { get; private set; }
    [field: SerializeField] public FollowerCircleState FollowerCircleState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        FollowerChaseState.Init(this);
        EnemyChaseState = FollowerChaseState;
        FollowerWanderState.Init(this);
        FollowerCircleState.Init(this);
        FollowerAttackState.Init(this);
        FollowerReadyAttackState.Init(this);
        FollowerAttackRecoverState.Init(this);
    }
    #endregion

    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

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
        EndHit();
    }

    public void StartHit()
    {
        FollowerAttackState.Weapon.EnableTriggers();
    }

    public void EndHit()
    {
        FollowerAttackState.Weapon.DisableTriggers();
    }
}
