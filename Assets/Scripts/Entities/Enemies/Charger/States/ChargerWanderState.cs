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

            currentWanderDestination = GetRandomWanderPoint();
            charger.SetDestination(currentWanderDestination, true);
        }

        charger.MoveTowardsDestination();
        charger.SetSpeedModifier(CloseToPoint(currentWanderDestination, 0.05f) ? 0f : 1f);

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

    private bool CloseToPoint(Vector3 point, float error)
    {
        return charger.Distance(point) < error;
    }

    private Vector3 GetRandomWanderPoint()
    {
        // Raycast downwards to prevent charger from not wandering at all because it cannot reach
        RaycastHit raycastHit;

        int numTries = 16;

        for(int i = 0; i < numTries; i++)
        {
            float randomRadius = Random.Range(charger.WanderRadiusRange.x, charger.WanderRadiusRange.y);

            Vector3 randomPointOnUnitCircle = Random.onUnitSphere;
            randomPointOnUnitCircle.y = 0;

            Vector3 randomPoint = randomRadius * randomPointOnUnitCircle + charger.transform.position;

            bool isValidPoint = 
                Physics.Raycast(randomPoint + 10f * Vector3.up, Vector3.down, out raycastHit, Mathf.Infinity, LayerMask.GetMask("Ground"))
                && NavMesh.SamplePosition(raycastHit.point, out _, 0.5f, NavMesh.AllAreas);
            
            if(isValidPoint) return raycastHit.point;
        }

        return charger.transform.position;
    }
}