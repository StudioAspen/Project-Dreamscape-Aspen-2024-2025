using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.AI;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class LeaperWanderState : LeaperBaseState
{
    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    public override void OnEnter()
    {
        enemy.TransitionToAnimation("FlatMovement");

        leaper.SetSpeedModifier(0f);

        wanderTimeElapsed = 0;
        randomWanderIntervalDuration = Random.Range(leaper.WanderIntervalDurationRange.x, leaper.WanderIntervalDurationRange.y);
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        leaper.ApplyGravity();

        if (leaper.IsGrounded)
        {
            if (leaper.Target != null) // once target is discovered
            {
                leaper.LeaperChaseState.AssignCurrentRememberedTarget(leaper.Target);
                leaper.ChangeState(leaper.EnemyChaseState);
                return;
            }

            wanderTimeElapsed += leaper.LocalDeltaTime;
        }

        // if ready to hop
        if (wanderTimeElapsed > randomWanderIntervalDuration)
        {
            wanderTimeElapsed = 0f;
            randomWanderIntervalDuration = Random.Range(leaper.WanderIntervalDurationRange.x, leaper.WanderIntervalDurationRange.y);

            currentWanderDestination = leaper.GetRandomWanderPoint(leaper.WanderRadiusRange);

            leaper.Hop(currentWanderDestination, leaper.WanderHopHeight);

            leaper.TransitionToAnimation("JumpingUp");
        }
        
        leaper.SetSpeedModifier(leaper.IsGrounded ? 0f : 1f);

        if (!leaper.IsGrounded)
        {
            leaper.LookAt(currentWanderDestination);

            leaper.ApplyHorizontalVelocity();
        }
    }
}