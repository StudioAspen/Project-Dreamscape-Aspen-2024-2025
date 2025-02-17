using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShielderDefensiveState : EnemyChaseState
{
    private Shielder shielder;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        shielder = entity as Shielder;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        enemy.TransitionToAnimation("DefensiveWalk");
        shielder.SetSpeedModifier(.5f);

    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Vector3 attackDir = shielder.Target.transform.position - shielder.transform.position;

        if (shielder.Target == null)
        {
            shielder.ChangeState(shielder.ShielderIdleState);
            return;
        }

        shielder.ShielderFlyingState.SetAttackDirection(attackDir);

        if (shielder.Distance(shielder.Target) < shielder.ShielderShieldBashState.AttackRange)
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
