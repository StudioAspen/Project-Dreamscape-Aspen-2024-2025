using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Shielder : Enemy
{
    [field: Header("Shielder: Equipment")]
    [field: SerializeField] public Weapon LongSword { get; protected set; }
    [field: SerializeField] public Weapon Shield { get; protected set; }

    #region States
    [field: Header("Shielder: States")]
    [field: SerializeField] public ShielderWanderState ShielderWanderState { get; private set; }
    [field: SerializeField] public ShielderTargetDetectedState ShielderTargetDetectedState { get; private set; }
    [field: SerializeField] public ShielderDefensiveState ShielderDefensiveState { get; private set; }
    [field: SerializeField] public ShielderQuickAttackState ShielderQuickAttackState { get; private set; }
    [field: SerializeField] public ShielderPowerAttack ShielderPowerAttackState { get; private set; }
    [field: SerializeField] public ShielderFlyingState ShielderFlyingState { get; private set; }
    [field: SerializeField] public ShielderShieldBashState ShielderShieldBashState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        ShielderWanderState.Init(this);
        ShielderTargetDetectedState.Init(this);
        ShielderDefensiveState.Init(this);
        ShielderQuickAttackState.Init(this);
        ShielderPowerAttackState.Init(this);
        ShielderFlyingState.Init(this);
        ShielderShieldBashState.Init(this);
    }
    #endregion

    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    private protected override void OnStart()
    {
        base.OnStart();

        SetDefaultState(ShielderWanderState);
    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    private protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public void StartHit()
    {
        LongSword.EnableTriggers();
        Shield.EnableTriggers();
    }

    public void EndHit()
    {
        LongSword.DisableTriggers();
        Shield.DisableTriggers();
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
        }
    }
}