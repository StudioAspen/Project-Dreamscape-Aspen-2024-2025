using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : Enemy
{
    [field: SerializeField] public float ShielderStunTime { get; private set; } = 1f;

    [field: SerializeField] public Weapon LongSword { get; private set; }
    [field: SerializeField] public Weapon Shield { get; private set; }

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
        ShielderQuickAttackState.Weapon.EnableTriggers();
    }

    public void EndHit()
    {
        ShielderQuickAttackState.Weapon.DisableTriggers();
    }


    public void CheckCollisions(float damagePercent, ref List<Entity> hitEntities)
    {
        List<Collider> hits = GetCustomCollisionHits(ShielderFlyingState.ShielderFlyingLayerMask);

        foreach (Collider hit in hits)
        {
            if (DidHitEnemyEntity(hit, out Entity enemyEntity))
            {
                if (hitEntities.Contains(enemyEntity)) continue;
                hitEntities.Add(enemyEntity);

                DealDamageToOtherEntity(enemyEntity, CalculateDamage(damagePercent), hit.ClosestPoint(GetColliderCenterPosition()));
            }
        }
    }

}
