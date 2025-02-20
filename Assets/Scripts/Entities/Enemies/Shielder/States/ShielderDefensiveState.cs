using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class ShielderDefensiveState : ShielderBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float SpeedModifier { get; private set; } = 0.5f;

    public override void OnEnter()
    {
        enemy.PlayOneShotAnimation(AnimationClip);

        shielder.SetSpeedModifier(SpeedModifier);

        shielder.Shield.EnableTriggers();
    }

    public override void OnExit()
    {
        shielder.Shield.DisableTriggers();
    }

    public override void OnUpdate()
    {
        shielder.ApplyGravity();

        if(shielder.Target == null)
        {
            shielder.ChangeState(shielder.ShielderWanderState);
            return;
        }

        // Bash if too close
        if (shielder.Distance(shielder.Target) < shielder.ShielderShieldBashState.AttackRange)
        {
            Vector3 attackDirection = shielder.Target.transform.position - shielder.transform.position;
            shielder.ShielderShieldBashState.SetAttackDirection(attackDirection);
            shielder.ChangeState(shielder.ShielderShieldBashState);
            return;
        }

        if(shielder.Distance(shielder.Target) < shielder.ShielderQuickAttackState.AttackRange)
        {
            Vector3 attackDirection = shielder.Target.transform.position - shielder.transform.position;
            shielder.ShielderQuickAttackState.SetAttackDirection(attackDirection);
            shielder.ChangeState(shielder.ShielderQuickAttackState);
            return;
        }

        enemy.SetDestination(enemy.Target.transform.position);
        enemy.MoveTowardsDestination();
    }
}
