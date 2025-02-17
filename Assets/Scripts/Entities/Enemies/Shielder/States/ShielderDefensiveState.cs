using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ShielderDefensiveState : EnemyChaseState
{
    private Shielder shielder;
    private float timer;

    private List<Entity> entitiesHitByCurrentShield = new List<Entity>();

    [field: SerializeField] public Weapon Shield { get; protected set; }


    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        shielder = entity as Shielder;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        entitiesHitByCurrentShield.Clear();

        enemy.TransitionToAnimation("DefensiveWalk");

        timer = 0f;

        shielder.SetSpeedModifier(.5f);
        shielder.SetRotationModifier(1.5f);

        if (Shield != null && Shield.MainCollider != null)
        {
            Shield.MainCollider.enabled = true;
        }
    }

    public override void OnExit()
    {
        shielder.SetSpeedModifier(0f);
        shielder.SetRotationModifier(7f);


        if (Shield != null && Shield.MainCollider != null)
        {
            Shield.MainCollider.enabled = false;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        timer += shielder.LocalDeltaTime;

        Shield.ColliderAdjustment(2.5f, 1.6f);
        shielder.CheckCollisions(0, ref entitiesHitByCurrentShield);

        Vector3 attackDir = shielder.Target.transform.position - shielder.transform.position;

        //STATE CHANGING
        if (shielder.Target == null)
        {
            shielder.ChangeState(shielder.ShielderIdleState);
            return;
        }

        shielder.ShielderFlyingState.SetAttackDirection(attackDir);

        if (shielder.Distance(shielder.Target) < shielder.ShielderShieldBashState.AttackRange && timer > shielder.ShieldBashInterval)
        {
            shielder.ShielderShieldBashState.SetAttackDirection(attackDir);
            shielder.ChangeState(shielder.ShielderShieldBashState);
            return;
        }


        //if shielder shield weapon gets hit by shielder.Target.Weapon

        if (!shielder.IsCurrentPathValid())
        {
            return;
        }
    }
}
