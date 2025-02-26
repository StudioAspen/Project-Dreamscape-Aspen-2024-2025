using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class ShielderWanderState : ShielderBaseState
{
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(3f, 5f);

    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    public override void OnEnter()
    {
        enemy.PlayDefaultAnimation();

        shielder.SetSpeedModifier(1f);

        shielder.ClearTarget();

        wanderTimeElapsed = Mathf.Infinity;
        randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);
    }

    public override void OnExit()
    {
        shielder.CancelPath();
    }

    public override void OnUpdate()
    {
        shielder.ApplyGravity();

        wanderTimeElapsed += shielder.LocalDeltaTime;
        shielder.TryAssignTarget();

        if (!shielder.IsCurrentPathValid())
        {
            SetNewWanderDestination();
        }

        if (wanderTimeElapsed > randomWanderIntervalDuration)
        {
            SetNewWanderDestination();
        }
        shielder.MoveTowardsDestination();
        shielder.SetSpeedModifier(shielder.CloseToPoint(currentWanderDestination) ? 0f : 1f);

        if (shielder.Target != null)
        {
            shielder.ChangeState(shielder.ShielderDefensiveState);
            return;
        }
    }

    /// <summary>
    /// Sets a new wander destination for the entity;
    /// </summary>
    private void SetNewWanderDestination()
    {
        wanderTimeElapsed = 0f;
        randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);

        currentWanderDestination = shielder.GetRandomWanderPoint(WanderRadiusRange);
        shielder.SetDestination(currentWanderDestination);
    }
}
