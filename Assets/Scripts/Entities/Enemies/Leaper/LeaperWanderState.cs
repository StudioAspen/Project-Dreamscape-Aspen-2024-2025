using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.AI;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class LeaperWanderState : EnemyBaseState
{
    private Leaper leaper;

    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    private List<Entity> entitiesHitByCurrentLeap = new List<Entity>();

    public LeaperWanderState(Leaper enemy) : base(enemy)
    {
        leaper = enemy;
    }

    public override void OnEnter()
    {
        enemy.TransitionToAnimation("FlatMovement");

        leaper.SetSpeedModifier(0f);

        wanderTimeElapsed = Mathf.Infinity;
        randomWanderIntervalDuration = Random.Range(leaper.WanderIntervalDurationRange.x, leaper.WanderIntervalDurationRange.y);

        entitiesHitByCurrentLeap.Clear();

        leaper.OnGrounded.AddListener(Leaper_OnGrounded);
    }

    public override void OnExit()
    {
        leaper.OnGrounded.RemoveListener(Leaper_OnGrounded);
    }

    public override void Update()
    {
        leaper.ApplyGravity();

        if(leaper.IsGrounded) wanderTimeElapsed += leaper.LocalDeltaTime;

        // if ready to hop
        if (wanderTimeElapsed > randomWanderIntervalDuration)
        {
            wanderTimeElapsed = 0f;
            randomWanderIntervalDuration = Random.Range(leaper.WanderIntervalDurationRange.x, leaper.WanderIntervalDurationRange.y);

            entitiesHitByCurrentLeap.Clear();

            currentWanderDestination = leaper.GetRandomWanderPoint(leaper.WanderRadiusRange);

            leaper.Hop(currentWanderDestination, leaper.WanderHopHeight);

            leaper.TransitionToAnimation("JumpingUp");

            leaper.CreateTempHitVisual(currentWanderDestination, 1f, Color.red, 5f);
        }
        
        leaper.SetSpeedModifier(leaper.IsGrounded ? 0f : 1f);

        if (!leaper.IsGrounded)
        {
            leaper.LookAt(currentWanderDestination);

            leaper.ApplyHorizontalVelocity();

            leaper.CheckCollisions(ref entitiesHitByCurrentLeap);
        }

        if (leaper.Target != null)
        {
            //leaper.LeaperTargetDetectedState.AssignCurrentRememberedTarget(leaper.Target);
            //leaper.ChangeState(leaper.EnemyChaseState);
            return;
        }
    }

    public override void FixedUpdate()
    {
    
    }

    private void Leaper_OnGrounded(Vector3 groundedPosition)
    {
        leaper.TransitionToAnimation("FlatMovement");
    }
}