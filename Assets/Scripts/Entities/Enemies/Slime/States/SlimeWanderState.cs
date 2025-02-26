using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlimeWanderState : SlimeBaseState
{
    [field: SerializeField] public Vector2 WanderIntervalDurationRange {get;private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 WanderRadiusRange {get;private set;} = new Vector2(3f, 5f);
    [field: SerializeField] public float WanderHopHeight {get;private set;} = 2f;
    
    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    public override void OnEnter()
    {
        slime.PlayDefaultAnimation();

        slime.SetSpeedModifier(0);

        wanderTimeElapsed = 0;
        randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        slime.ApplyGravity();
        
        if (slime.IsGrounded)
        {
            if (slime.Target != null)
            {
                slime.SlimeChaseState.AssignCurentRememberedTarget(slime.Target);
                slime.ChangeState(slime.SlimeChaseState);
                return;
            }
            wanderTimeElapsed += slime.LocalDeltaTime;
        }

        if (wanderTimeElapsed > randomWanderIntervalDuration)
        {
            wanderTimeElapsed = 0f;
            randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);
            
            currentWanderDestination = slime.GetRandomWanderPoint(WanderRadiusRange);

            slime.Hop(currentWanderDestination, WanderHopHeight);

            slime.PlayOneShotAnimation(slime.JumpAnimationClip);
        }

        slime.SetSpeedModifier(slime.IsGrounded ? 0f : 1f);

        if (!slime.IsGrounded)
        {
            slime.LookAt(currentWanderDestination);

            slime.ApplyHorizontalVelocity();
        }
    }
}
