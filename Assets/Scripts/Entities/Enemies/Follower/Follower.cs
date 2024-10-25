using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Enemy
{
    [field: Header("Follower: Settings")]
    [field: SerializeField] public int CircleEntityCountThreshold { get; private set; } = 2;

    #region States
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
    }
}
