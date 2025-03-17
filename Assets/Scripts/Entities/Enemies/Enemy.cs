using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    [field: Header("Enemy: Settings")]
    [field: SerializeField] public int Cost { get; protected set; }
    [field: SerializeField] public Stat EXPValue { get; protected set; }

    [field: Header("Enemy: Custom Collider Settings")]
    [field: SerializeField] public float CustomCollisionRadius { get; private set; }
    [field: SerializeField] public float CustomCollisionOffsetFromGroundDistance { get; private set; } = 0.5f;
    [field: SerializeField] public Vector3 CustomCollisionCenterOffset { get; private set; }
    public Vector3 ChargeCollisionBottomPoint => GetColliderCenterPosition() + transform.localScale.x * (Quaternion.LookRotation(transform.forward) * CustomCollisionCenterOffset) - transform.localScale.x * (CharacterController.height / 2 - CustomCollisionRadius - CustomCollisionOffsetFromGroundDistance) * Vector3.up;
    public Vector3 CustomCollisionTopPoint => GetColliderCenterPosition() + transform.localScale.x * (Quaternion.LookRotation(transform.forward) * CustomCollisionCenterOffset) + transform.localScale.x * (CharacterController.height / 2 -  CustomCollisionRadius) * Vector3.up;

    #region Custom Pathfinding
    public Vector3 Destination {  get; protected set; }
    private List<Vector3> path;
    #endregion

    public Entity Target { get; protected set; }

    public EnemySpawner Spawner { get; private set; }

    #region States
    [field: Header("Enemy: States")]
    [field: SerializeField] public EnemyIdleState EnemyIdleState { get; protected set; }
    [field: SerializeField] public EnemyChaseState EnemyChaseState { get; protected set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        EnemyIdleState.Init(this);
        EnemyChaseState.Init(this);
    }
    #endregion

    /// <summary>
    /// Initializes the enemy with the specified enemy spawner.
    /// Used to delete the enemy from the spawner's list when it dies.
    /// </summary>
    /// <param name="enemySpawner">The enemy spawner.</param>
    public void Init(EnemySpawner enemySpawner)
    {
        Spawner = enemySpawner;
    }

    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

        if (Ticker.Instance != null) Ticker.Instance.OnTick += OnTick;

        SetStartState(EntitySpawnState);

        Target = null;
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();

        if (Ticker.Instance != null) Ticker.Instance.OnTick -= OnTick;
    }

    private protected override void OnStart()
    {
        base.OnStart();

        ChangeTeam(1);

        SetDefaultState(EnemyIdleState);
    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    private protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    private protected override void OnDeath()
    {
        base.OnDeath();
    }

    private protected override void OnOnDrawGizmos()
    {
        base.OnOnDrawGizmos();

        if(CustomCollisionRadius <= 0) return;

#if UNITY_EDITOR
        Gizmos.color = Color.white;
        CustomDebug.DrawWireCapsule(ChargeCollisionBottomPoint, CustomCollisionTopPoint, CustomCollisionRadius * transform.localScale.x);
#endif
    }

    public override void Die()
    {
        base.Die();

        if (Spawner != null) Spawner.RemoveEnemy(this);
    }

    /// <summary>
    /// Called every tick from the Ticker singleton.
    /// Tries to assign a target to the enemy here.
    /// </summary>
    private protected virtual void OnTick()
    {
        TryAssignTarget();
    }

    /// <summary>
    /// Calculates the path from the current position to the destination using NavMesh.
    /// </summary>
    /// <param name="dest">The destination position.</param>
    /// <returns>The list of positions representing the calculated path.</returns>
    public List<Vector3> GetPathToDestination(Vector3 dest)
    {
        if(!IsValidPointOnNavMesh(dest, 100f, out Vector3 groundedPoint))
        {
            return new List<Vector3>() { transform.position, dest };
        }

        NavMeshPath path = new NavMeshPath();

        bool hasPath = NavMesh.CalculatePath(transform.position, groundedPoint, NavMesh.AllAreas, path);

        if (!hasPath) return null;
        if (path.corners.Length == 0) return null;

        return path.corners.ToList();
    }

    /// <summary>
    /// Moves the enemy towards its destination along the calculated path.
    /// Looks at the path by default.
    /// /// <param name="lookAtPath">Whether to look at the path.</param>
    /// </summary>
    public void MoveTowardsDestination(bool lookAtPath = true)
    {
        if (path == null)
        {
            Debug.LogWarning($"{gameObject.name} has no path to follow. Make sure you handle this using bool IsCurrentPathValid().");
            return;
        }
        if (path.Count < 2)
        {
            Debug.LogWarning($"{gameObject.name} has a path with only 1 waypoint. Make sure you handle this bool IsCurrentPathValid().");
            return;
        }

        #region Debug
/*        Vector3 prevCorner = transform.position;
        foreach (Vector3 wayPoint in path)
        {
            Debug.DrawLine(prevCorner, wayPoint, Color.green);
            prevCorner = wayPoint;
        }*/
        #endregion

        Vector3 currentDestination = path[1];

        Vector3 direction = (currentDestination - transform.position).normalized;
        //direction = CalculateDirectionAwayFromObstacle(direction);

        if (lookAtPath) LookAt(transform.position + direction);

        UpdateHorizontalVelocity(direction);
        ApplyHorizontalVelocity();

        if (Vector3.Dot(direction, (path[1] - path[0]).normalized) < 0 || CloseToPoint(currentDestination, 0.05f))
        {
            path.RemoveAt(0);
        }
    }

    /// <summary>
    /// Checks if the current path of the enemy is valid.
    /// </summary>
    /// <returns>True if the path is valid, false otherwise.</returns>
    public bool IsCurrentPathValid()
    {
        if (path == null) return false;
        if (path.Count < 2) return false;

        return true;
    }

    /// <summary>
    /// Cancels the current path of the enemy.
    /// </summary>
    public void CancelPath()
    {
        path = null;
    }

    /// <summary>
    /// Sets the destination for the enemy to move towards.
    /// You can specify whether you want the enemy to look at the path while moving.
    /// </summary>
    /// <param name="dest">The destination position.</param>
    /// <param name="lookAtPath">Whether the enemy should look at the path while moving.</param>
    public void SetDestination(Vector3 dest)
    {
        Destination = dest;
        path = GetPathToDestination(dest);
    }

    /// <summary>
    /// Checks if a given point is on the NavMesh using raycast from maxHeight above the point.
    /// Also returns the valid point on the NavMesh or the enemy's current position if not valid.
    /// The y value doesn't matter, and the point will always be checked at the navmesh ground level.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <param name="maxHeight">The max height above the point to start raycasting down from.</param>
    /// <param name="outputPoint">The output point.</param>
    /// <returns>True if the point is on the NavMesh, false otherwise.</returns>
    public bool IsValidPointOnNavMesh(Vector3 point, float maxHeight, out Vector3 outputPoint)
    {
        RaycastHit raycastHit;
        bool isValidPoint =
            Physics.Raycast(point + maxHeight * Vector3.up, Vector3.down, out raycastHit, Mathf.Infinity, LayerMask.GetMask("Ground"))
            && NavMesh.SamplePosition(raycastHit.point, out _, 0.5f, NavMesh.AllAreas);

        outputPoint = isValidPoint ? raycastHit.point : transform.position;

        return isValidPoint;
    }

    /// <summary>
    /// Generates a random wander point within a specified radius range.
    /// Returns itself it cannot find a valid point after a certain number of iterations.
    /// Iteration count is set to 16 by default.
    /// </summary>
    /// <param name="wanderRadiusRange">The range of the wander radius.</param>
    /// <param name="iterationTryCount">The number of iterations to try finding a valid point.</param>
    /// <returns>The randomly generated wander point.</returns>
    public Vector3 GetRandomWanderPoint(Vector2 wanderRadiusRange, int iterationTryCount = 16)
    {
        for (int i = 0; i < iterationTryCount; i++)
        {
            float randomRadius = Random.Range(wanderRadiusRange.x, wanderRadiusRange.y);
            Vector3 randomPointOnUnitCircle = Random.onUnitSphere;
            randomPointOnUnitCircle.y = 0;
            Vector3 randomPoint = randomRadius * randomPointOnUnitCircle + transform.position;

            if (IsValidPointOnNavMesh(randomPoint, 100f, out Vector3 validPoint)) return validPoint;
        }

        return transform.position;
    }

    /// <summary>
    /// Generates a random wander point within a specified radius range about a center position.
    /// Returns itself it cannot find a valid point after a certain number of iterations.
    /// Iteration count is set to 16 by default.
    /// </summary>
    /// <param name="center">The center position for generating the wander point.</param>
    /// <param name="wanderRadiusRange">The range of the wander radius.</param>
    /// <param name="iterationTryCount">The number of iterations to try finding a valid point.</param>
    /// <returns>The randomly generated wander point.</returns>
    public Vector3 GetRandomWanderPoint(Vector3 center, Vector2 wanderRadiusRange, int iterationTryCount = 16)
    {
        for (int i = 0; i < iterationTryCount; i++)
        {
            float randomRadius = Random.Range(wanderRadiusRange.x, wanderRadiusRange.y);
            Vector3 randomPointOnUnitCircle = Random.onUnitSphere;
            randomPointOnUnitCircle.y = 0;
            Vector3 randomPoint = randomRadius * randomPointOnUnitCircle + center;

            if (IsValidPointOnNavMesh(randomPoint, 100f, out Vector3 validPoint)) return validPoint;
        }

        return transform.position;
    }

    /// <summary>
    /// Tries to assign a target to the enemy by getting nearby targets and selecting the first one.
    /// If no targets are found, sets the target to null.
    /// </summary>
    public virtual void TryAssignTarget()
    {
        List<Entity> targets = GetNearbyTargets();
        if (targets.Count == 0)
        {
            Target = null;
            return;
        }

        Target = targets[0];
    }

    /// <summary>
    /// Tries to assign a target to the enemy within a cone-shaped detection area.
    /// Override TryAssignTarget() and call this function to use cone-shaped detection.
    /// </summary>
    /// <param name="detectionDistance">The distance of the detection area.</param>
    /// <param name="detectionConeHalfAngle">Half of the total angle of the detection cone.</param>
    private protected void TryAssignTargetWithCone(float detectionDistance, float detectionConeHalfAngle)
    {
        List<Entity> smallRadiusTargets = GetNearbyTargets();
        List<Entity> largeRadiusTargets = GetNearbyHostileEntities(detectionDistance, false);
        List<Entity> filteredTargetsByCone = FilterTargetsInConeShape(largeRadiusTargets, CustomCollisionTopPoint, detectionConeHalfAngle);

        if (largeRadiusTargets.Count == 0) // if no targets in large radius that includes cone
        {
            Target = null;
            return;
        }

        if (filteredTargetsByCone.Count > 0 && !IsBlockedFromEntity(filteredTargetsByCone[0])) // if no targets in cone that are not blocked
        {
            Target = filteredTargetsByCone[0];
            return;
        }

        if (smallRadiusTargets.Count > 0) // if no targets in cone, but there are targets in very small radius
        {
            Target = smallRadiusTargets[0];
            return;
        }

        Target = null;
        return;
    }

    /// <summary>
    /// Filters targets in a cone shape starting from the center of the collider with a total angle of 2 * coneHalfAngle.
    /// </summary>
    /// <param name="targets">The list of targets to filter.</param>
    /// <param name="center">The center position of the collider.</param>
    /// <param name="coneHalfAngle">Half of the total angle of the cone.</param>
    /// <returns>The filtered list of targets.</returns>
    public virtual List<Entity> FilterTargetsInConeShape(List<Entity> targets, Vector3 center, float coneHalfAngle)
    {
        if (targets.Count == 0) return targets;

        List<Entity> filteredTargets = new List<Entity>();

        Vector3 forwardDirection = transform.forward;

        foreach (Entity target in targets)
        {
            Vector3 directionToTarget = target.GetColliderCenterPosition() - center;

            // Calculate the angle between the forward direction and the direction to the target
            float angle = Vector3.Angle(forwardDirection, directionToTarget.normalized);

            // If angle is within half of the cone's angle, add to filtered targets
            if (angle <= coneHalfAngle) filteredTargets.Add(target);
        }

        return filteredTargets;
    }

    /// <summary>
    /// Gets the colliders that overlap with the custom collision capsule of the enemy.
    /// Filters out the enemy's own colliders.
    /// The list will be ordered by ascending distance by default.
    /// </summary>
    /// <param name="mask">The layer mask to filter the colliders.</param>
    /// <param name="isOrderedByAscendingDistance">Whether to order the colliders by ascending distance.</param>
    /// <returns>The list of colliders that overlap with the custom collision capsule.</returns>
    public List<Collider> GetCustomCollisionHits(LayerMask mask, bool isOrderedByAscendingDistance = true)
    {
        List<Collider> result = new List<Collider>();

        if (CustomCollisionRadius <= 0) return result;

        Collider[] hits = Physics.OverlapCapsule(ChargeCollisionBottomPoint, CustomCollisionTopPoint, CustomCollisionRadius * transform.localScale.x, mask);
        if (hits == null) return result;
        if (hits.Length == 0) return result;

        foreach(Collider hit in hits)
        {
            if(IsOwnCollider(hit)) continue; // filter out own colliders

            result.Add(hit);
        }

        return isOrderedByAscendingDistance ? result.OrderBy(hit => Distance(hit.ClosestPoint(GetColliderCenterPosition() + CustomCollisionCenterOffset))).ToList() : result.ToList();
    }

    public void ClearTarget()
    {
        Target = null;
    }
}