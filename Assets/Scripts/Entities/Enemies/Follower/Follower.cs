using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Enemy
{
    #region States
    public FollowerAttackState FollowerAttackState { get; private set; }
    public FollowerWanderState FollowerWanderState { get; private set; }
    public FollowerReadyAttackState FollowerReadyAttackState { get; private set; }
    public FollowerAttackRecoverState FollowerAttackRecoverState { get; private set; }
    public FollowerCircleState FollowerCircleState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        EnemyChaseState = EntityBaseState.InitializeOrCreate<FollowerChaseState>(this);
        FollowerWanderState = EntityBaseState.InitializeOrCreate<FollowerWanderState>(this);
        FollowerCircleState = EntityBaseState.InitializeOrCreate<FollowerCircleState>(this);
        FollowerAttackState = EntityBaseState.InitializeOrCreate<FollowerAttackState>(this);
        FollowerReadyAttackState = EntityBaseState.InitializeOrCreate<FollowerReadyAttackState>(this);
        FollowerAttackRecoverState = EntityBaseState.InitializeOrCreate<FollowerAttackRecoverState>(this);
    }
    #endregion

    private protected override void OnAwake()
    {
        base.OnAwake();
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
        FollowerAttackState.Weapon.EnableTriggers();
    }

    public void DisableWeaponTriggers()
    {
        FollowerAttackState.Weapon.DisableTriggers();
    }
}
