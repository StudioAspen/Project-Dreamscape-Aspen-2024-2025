using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class ShielderPlayerDetectState : EnemyChaseState 
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


        //Attack Sequence.
        if ((shielder.Distance(shielder.Target) < shielder.ShielderPowerAttackState.AttackRange) && (shielder.Target.CurrentState == shielder.Target.EntityLaunchState))
        {
            //Vector3 attackDir = shielder.Target.transform.position - shielder.transform.position;
            shielder.ShielderPowerAttackState.SetAttackDirection(attackDir);
            shielder.ChangeState(shielder.ShielderPowerAttackState);
            return;

        }
        else if (shielder.Distance(shielder.Target) < shielder.ShielderQuickAttackState.AttackRange)
        {
            //Vector3 attackDir = shielder.Target.transform.position - shielder.transform.position;
            shielder.ShielderQuickAttackState.SetAttackDirection(attackDir);
            shielder.ChangeState(shielder.ShielderQuickAttackState);
            return;
        }

        if (!shielder.IsCurrentPathValid())
        {
            return;
        }
    }

}

