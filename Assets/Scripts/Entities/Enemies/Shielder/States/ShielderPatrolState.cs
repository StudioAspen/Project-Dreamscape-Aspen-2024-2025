using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ShielderPatrolState : EnemyBaseState
{
    private Shielder shielder;

    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    public ShielderPatrolState(Shielder enemy) : base(enemy)
    {
        shielder = enemy;
    }
    public override void OnEnter()
    {
        shielder.DefaultTransitionToAnimation("FlatMovement");

        shielder.SetSpeedModifier(1f);

        shielder.ClearTarget();

        wanderTimeElapsed = Mathf.Infinity;
        randomWanderIntervalDuration = Random.Range(shielder.WanderIntervalDurationRange.x, shielder.WanderIntervalDurationRange.y);
    }

    public override void OnExit()
    {
        shielder.CancelPath();
    }

    public override void Update()
    {
        wanderTimeElapsed += Time.deltaTime;

        shielder.TryAssignTarget();

        if (wanderTimeElapsed > randomWanderIntervalDuration)
        {
            wanderTimeElapsed = 0f;
            randomWanderIntervalDuration = Random.Range(shielder.WanderIntervalDurationRange.x, shielder.WanderIntervalDurationRange.y);

            currentWanderDestination = GetRandomWanderPoint();
            shielder.SetDestination(currentWanderDestination, true);
        }

        shielder.SetSpeedModifier(CloseToPoint(currentWanderDestination, 0.05f) ? 0f : 1f);

        if (shielder.Target != null)
        {
            shielder.ShielderPlayerDetectedState.AssignCurrentRememberedTarget(shielder.Target);
            shielder.ChangeState(shielder.ShielderPlayerDetectedState);
            return;
        }
    }

    public override void FixedUpdate() { }

    // Charger also uses this, maybe make this a public function for entity or enemy?
    private bool CloseToPoint(Vector3 point, float error)
    {
        return shielder.Distance(point) < error;
    }

    private Vector3 GetRandomWanderPoint()
    {
        // Raycast downwards to prevent charger from not wandering at all because it cannot reach
        RaycastHit raycastHit;

        int numTries = 16;

        for (int i = 0; i < numTries; i++)
        {
            float randomRadius = Random.Range(shielder.WanderRadiusRange.x, shielder.WanderRadiusRange.y);

            Vector3 randomPointOnUnitCircle = Random.onUnitSphere;
            randomPointOnUnitCircle.y = 0;

            Vector3 randomPoint = randomRadius * randomPointOnUnitCircle + shielder.transform.position;

            bool isValidPoint =
                Physics.Raycast(randomPoint + 10f * Vector3.up, Vector3.down, out raycastHit, Mathf.Infinity, LayerMask.GetMask("Ground"))
                && NavMesh.SamplePosition(raycastHit.point, out _, 0.5f, NavMesh.AllAreas);

            if (isValidPoint) return raycastHit.point;
        }

        return shielder.transform.position;
    }
}