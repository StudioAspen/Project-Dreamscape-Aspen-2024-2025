using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChargerWanderState : EnemyBaseState
{
    private Charger charger;

    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    public ChargerWanderState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }
    public override void OnEnter()
    {
        enemy.TransitionToAnimation("FlatMovement");

        charger.SetSpeedModifier(1f);

        charger.ClearTarget();

        wanderTimeElapsed = Mathf.Infinity;
        randomWanderIntervalDuration = Random.Range(charger.WanderIntervalDurationRange.x, charger.WanderIntervalDurationRange.y);
    }

    public override void OnExit()
    {
        charger.CancelPath();
    }

    public override void Update()
    {
        charger.ApplyGravity();

        wanderTimeElapsed += charger.LocalDeltaTime;

        charger.TryAssignTarget();

        if(wanderTimeElapsed > randomWanderIntervalDuration)
        {
            wanderTimeElapsed = 0f;
            randomWanderIntervalDuration = Random.Range(charger.WanderIntervalDurationRange.x, charger.WanderIntervalDurationRange.y);

            currentWanderDestination = charger.GetRandomWanderPoint(charger.WanderRadiusRange);
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

    public override void FixedUpdate()
    {
        
    }
}