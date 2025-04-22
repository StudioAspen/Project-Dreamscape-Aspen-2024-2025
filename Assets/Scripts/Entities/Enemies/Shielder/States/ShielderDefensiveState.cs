using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class ShielderDefensiveState : ShielderBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float AnimationSpeed { get; private set; } = 0.75f;
    [field: SerializeField] public float SpeedModifier { get; private set; } = 0.5f;
    [field: SerializeField] public int StaggerDamageThreshold { get; private set; } = 20;

    public override void OnEnter()
    {
        enemy.PlayOneShotAnimation(AnimationClip, AnimationClip.length / AnimationSpeed);

        shielder.SetSpeedModifier(SpeedModifier);

        shielder.Shield.EnableTriggers(false); // Will not do damage but still listen for hits

        shielder.OnEntityTakeDamage += Shielder_OnEntityTakeDamage;
    }

    public override void OnExit()
    {
        shielder.Shield.DisableTriggers();
        shielder.OnEntityTakeDamage -= Shielder_OnEntityTakeDamage;
    }

    public override void OnUpdate()
    {
        shielder.ApplyGravity();

        if(shielder.Target == null)
        {
            shielder.ChangeState(shielder.ShielderWanderState);
            return;
        }

        if(shielder.Distance(shielder.Target) < shielder.ShielderQuickAttackState.AttackRange)
        {
            if (shielder.Target.CurrentState == shielder.Target.EntityLaunchState || shielder.Target.CurrentState == shielder.Target.EntityStunnedState)
            {
                shielder.ChangeState(shielder.ShielderPowerAttackState);
                return;
            }

            Vector3 attackDirection = shielder.Target.transform.position - shielder.transform.position;
            // Chance to shield bash 1/2
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                shielder.ShielderShieldBashState.SetAttackDirection(attackDirection);
                shielder.ChangeState(shielder.ShielderShieldBashState);
            }
            else
            {
                shielder.ShielderQuickAttackState.SetAttackDirection(attackDirection);
                shielder.ChangeState(shielder.ShielderQuickAttackState);
            }
            return;
        }

        enemy.SetDestination(enemy.Target.transform.position);
        enemy.MoveTowardsDestination();
    }

    private void Shielder_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        if (source == null) return;

        if(!source.TryGetComponent(out Entity attacker))
        {
            return;
        }
        
        if(damage >= StaggerDamageThreshold)
        {
            shielder.ChangeState(shielder.EntityStaggeredState);
            return;
        }

        attacker.ForceChangeToLaunchState(shielder, Vector3.zero, 0, shielder.ShielderShieldBashState.ShieldBashStunTime);
        shielder.ChangeState(shielder.ShielderPowerAttackState);
    }
}
