using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EscortEventEntityWanderState : EntityBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(7f, 10f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(7f, 10f);

    private EscortEventEntity escortEventEntity;
    private WorldManager worldManager;

    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        escortEventEntity = entity as EscortEventEntity;
        worldManager = GameObject.FindObjectOfType<WorldManager>();
    }

    public override void OnEnter()
    {
        escortEventEntity.PlayDefaultAnimation();

        escortEventEntity.SetSpeedModifier(1f);

        wanderTimeElapsed = Mathf.Infinity;
        randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);
    }

    public override void OnExit()
    {
        escortEventEntity.CancelPath();
    }

    public override void OnUpdate()
    {
        escortEventEntity.ApplyGravity();

        wanderTimeElapsed += escortEventEntity.LocalDeltaTime;

        if (!escortEventEntity.IsCurrentPathValid()) SetNewWanderDestination();

        if (wanderTimeElapsed > randomWanderIntervalDuration || escortEventEntity.CloseToPoint(currentWanderDestination, 1f)) SetNewWanderDestination();

        escortEventEntity.MoveTowardsDestination();
        escortEventEntity.SetSpeedModifier(escortEventEntity.CloseToPoint(currentWanderDestination) ? 0f : 1f);
    }

    /// <summary>
    /// Sets a new wander destination for the escort event entity.
    /// </summary>
    private void SetNewWanderDestination()
    {
        wanderTimeElapsed = 0f;
        randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);

        currentWanderDestination = escortEventEntity.GetRandomWanderPoint(GetRandomAdjacentLand().transform.position, WanderRadiusRange);
        escortEventEntity.SetDestination(currentWanderDestination);
    }

    /// <summary>
    /// Returns a random adjacent land that is not the current land of the escort event entity.
    /// If the random land cannot be determined, the current land is returned.
    /// If the current land cannot be determined, a random land is returned.
    /// </summary>
    /// <returns>The random land.</returns>
    private LandManager GetRandomAdjacentLand()
    {
        LandManager currentLand = worldManager.GetLandByWorldPosition(escortEventEntity.transform.position);

        // Escort event entity is not on a land
        if(currentLand == null) return worldManager.GetRandomLand();

        List<LandManager> adjacentLands = GetAdjacentLands(currentLand);

        if(adjacentLands.Count == 0) return currentLand;

        return adjacentLands[Random.Range(0, adjacentLands.Count)];
    }

    /// <summary>
    /// Returns a list of adjacent lands to the given middle land.
    /// </summary>
    /// <param name="middleLand">The middle land.</param>
    /// <returns>The list of adjacent lands.</returns>
    private List<LandManager> GetAdjacentLands(LandManager middleLand)
    {
        List<LandManager> adjacentLands = new List<LandManager>();

        List<Vector2Int> adjacentDirections = new List<Vector2Int>
        {
            new Vector2Int(1, 0), // Right
            new Vector2Int(-1, 0), // Left
            new Vector2Int(0, 1), // Up
            new Vector2Int(0, -1) // Down
        };

        foreach(Vector2Int adjacentDirection in adjacentDirections)
        {
            if(worldManager.TryGetLandByGridPosition(middleLand.GridPosition + adjacentDirection, out LandManager adjacentLand))
            {
                adjacentLands.Add(adjacentLand);
            }
        }

        return adjacentLands;
    }
}
