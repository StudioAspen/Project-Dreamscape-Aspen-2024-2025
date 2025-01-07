using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EscortEventEntityWanderState : EntityBaseState
{
    private EscortEventEntity escortEventEntity;
    private WorldManager worldManager;

    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    public EscortEventEntityWanderState(EscortEventEntity escortEventEntity)
    {
        this.escortEventEntity = escortEventEntity;

        worldManager = GameObject.FindObjectOfType<WorldManager>();
    }

    public override void OnEnter()
    {
        escortEventEntity.TransitionToAnimation("FlatMovement");

        escortEventEntity.SetSpeedModifier(1f);

        wanderTimeElapsed = Mathf.Infinity;
        randomWanderIntervalDuration = Random.Range(escortEventEntity.WanderIntervalDurationRange.x, escortEventEntity.WanderIntervalDurationRange.y);
    }

    public override void OnExit()
    {
        escortEventEntity.CancelPath();
    }

    public override void Update()
    {
        escortEventEntity.ApplyGravity();

        wanderTimeElapsed += escortEventEntity.LocalDeltaTime;

        if (wanderTimeElapsed > randomWanderIntervalDuration || escortEventEntity.CloseToPoint(currentWanderDestination))
        {
            wanderTimeElapsed = 0f;
            randomWanderIntervalDuration = Random.Range(escortEventEntity.WanderIntervalDurationRange.x, escortEventEntity.WanderIntervalDurationRange.y);

            currentWanderDestination = escortEventEntity.GetRandomWanderPoint(GetRandomOtherLand().transform.position, escortEventEntity.WanderRadiusRange);
            escortEventEntity.SetDestination(currentWanderDestination);
        }

        escortEventEntity.MoveTowardsDestination();
        escortEventEntity.SetSpeedModifier(escortEventEntity.CloseToPoint(currentWanderDestination) ? 0f : 1f);
    }

    /// <summary>
    /// Returns a random land that is not the current land of the escort event entity.
    /// If the current land cannot be determined, a random land is returned.
    /// </summary>
    /// <returns>The random land.</returns>
    private LandManager GetRandomOtherLand()
    {
        List<LandManager> potentialLands = worldManager.SpawnedLands.Values.ToList();

        if (worldManager.TryGetLandByWorldPosition(escortEventEntity.transform.position, out LandManager currentLand))
        {
            potentialLands.Remove(currentLand);

            if (potentialLands.Count == 0)
                return worldManager.GetRandomLand();

            return potentialLands[Random.Range(0, potentialLands.Count)];
        }

        return worldManager.GetRandomLand();
    }
}
