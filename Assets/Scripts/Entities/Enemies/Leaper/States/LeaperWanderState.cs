using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

[System.Serializable]
public class LeaperWanderState : LeaperBaseState
{
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public float WanderHopHeight { get; private set; } = 2f;

    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    public override void OnEnter()
    {
        enemy.PlayDefaultAnimation();

        leaper.SetSpeedModifier(0f);

        wanderTimeElapsed = 0;
        randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);
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
            randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);

            currentWanderDestination = leaper.GetRandomWanderPoint(WanderRadiusRange);

            leaper.Hop(currentWanderDestination, WanderHopHeight);

            leaper.PlayOneShotAnimation(leaper.JumpAnimationClip);
        }
        
        leaper.SetSpeedModifier(leaper.IsGrounded ? 0f : 1f);

        if (!leaper.IsGrounded)
        {
            leaper.LookAt(currentWanderDestination);

            leaper.ApplyHorizontalVelocity();
        }
    }
}