using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Shielder : Enemy
{
    [field: Space]
    [field: SerializeField] public float ShielderStunTime { get; private set; } = 1f;
    [field: Space]
    [field: SerializeField] public float ShieldBashInterval { get; private set; } = 5f;
    [field: SerializeField] public float ShieldBashPushForce { get; private set; } = 5f;
    [field: SerializeField] public float ShieldBashStunTime { get; private set; } = 1f; 

    #region States
    public ShielderQuickAttackState ShielderQuickAttackState { get; private set; }
    public ShielderPowerAttack ShielderPowerAttackState { get; private set; }
    public ShielderIdleState ShielderIdleState { get; private set; }
    public ShielderPlayerDetectState ShielderPlayerDetectState { get; private set; }
    public ShielderFlyingState ShielderFlyingState { get; private set; }
    public ShielderDefensiveState ShielderDefensiveState { get; private set; }
    public ShielderShieldBashState ShielderShieldBashState { get; private set; }


    private protected override void InitializeStates()
    {
        base.InitializeStates();

        ShielderQuickAttackState = EntityBaseState.InitializeOrCreate<ShielderQuickAttackState>(this);
        ShielderPowerAttackState = EnemyBaseState.InitializeOrCreate<ShielderPowerAttack>(this);
        ShielderIdleState = EnemyBaseState.InitializeOrCreate<ShielderIdleState>(this);
        ShielderPlayerDetectState = EnemyBaseState.InitializeOrCreate<ShielderPlayerDetectState>(this);
        ShielderFlyingState = EnemyBaseState.InitializeOrCreate<ShielderFlyingState>(this);
        ShielderDefensiveState = EnemyBaseState.InitializeOrCreate<ShielderDefensiveState>(this);
        ShielderShieldBashState = EnemyBaseState.InitializeOrCreate<ShielderShieldBashState>(this);
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

        SetDefaultState(ShielderIdleState);

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
        ShielderQuickAttackState.LongSword.EnableTriggers();
        ShielderShieldBashState.Shield.EnableTriggers();
    }

    public void EndHit()
    {
        ShielderQuickAttackState.LongSword.DisableTriggers();
        ShielderShieldBashState.Shield.DisableTriggers();
    }


    public void CheckCollisions(float damagePercent, ref List<Entity> hitEntities)
    {
        List<Collider> hits = GetCustomCollisionHits(ShielderFlyingState.ShielderFlyingLayerMask);

        foreach (Collider hit in hits)
        {
            if (CurrentState == ShielderFlyingState)
            {
                if (DidHitEnemyEntity(hit, out Entity enemyEntity))
                {
                    if (hitEntities.Contains(enemyEntity)) continue;
                    hitEntities.Add(enemyEntity);

                    DealDamageToOtherEntity(enemyEntity, CalculateDamage(damagePercent), hit.ClosestPoint(GetColliderCenterPosition()));
                    return;
                }
            }

            if (CurrentState == ShielderShieldBashState)
            {
                if (DidHitEnemyEntity(hit, out Entity enemyEntity))
                {
                    if (ShielderShieldBashState.Shield.MainCollider == null || hit != ShielderShieldBashState.Shield.MainCollider)
                    {
                        if (hitEntities.Contains(enemyEntity)) continue;
                        hitEntities.Add(enemyEntity);

                        Vector3 launchDirection = enemyEntity.GetColliderCenterPosition() - transform.position;
                        enemyEntity.TryChangeToLaunchState(launchDirection, ShieldBashPushForce, ShieldBashStunTime);

                        return;
                    }
                }
            }
        }
    }
}