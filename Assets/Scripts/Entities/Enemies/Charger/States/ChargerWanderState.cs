using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChargerWanderState : ChargerBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(3f, 5f);

    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    public override void OnEnter()
    {
        enemy.TransitionToAnimation("FlatMovement");

        charger.SetSpeedModifier(1f);

        charger.ClearTarget();

        wanderTimeElapsed = Mathf.Infinity;
        randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);
    }

    public override void OnExit()
    {
        charger.CancelPath();
    }

    public override void OnUpdate()
    {
        charger.ApplyGravity();

        wanderTimeElapsed += charger.LocalDeltaTime;

        charger.TryAssignTarget();

        if(wanderTimeElapsed > randomWanderIntervalDuration)
        {
            wanderTimeElapsed = 0f;
            randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);

            currentWanderDestination = charger.GetRandomWanderPoint(WanderRadiusRange);
            charger.SetDestination(currentWanderDestination);
        }

        charger.MoveTowardsDestination();
        charger.SetSpeedModifier(charger.CloseToPoint(currentWanderDestination) ? 0f : 1f);

        if (charger.Target != null)
        {
            charger.ChargerTargetDetectedState.AssignCurrentRememberedTarget(charger.Target);
            charger.ChangeState(charger.ChargerTargetDetectedState);
            return;
        }
    }
}