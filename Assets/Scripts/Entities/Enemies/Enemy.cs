using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;
using UnityEngine.Pool;

public class Enemy : Entity
{
    [field : Header("Enemy: Settings")]
    [field: SerializeField] public int Cost { get; protected set; }

    [field: Header("Enemy: Custom Collider Settings")]
    [field: SerializeField] public float CustomCollisionRadius { get; private set; }
    [field: SerializeField] public float CustomCollisionOffsetFromGroundDistance { get; private set; } = 0.5f;
    [field: SerializeField] public Vector3 CustomCollisionCenterOffset { get; private set; }
    public Vector3 ChargeCollisionBottomPoint => GetColliderCenterPosition() + CustomCollisionCenterOffset - (controller.height / 2 - CustomCollisionRadius - CustomCollisionOffsetFromGroundDistance) * Vector3.up;
    public Vector3 CustomCollisionTopPoint => GetColliderCenterPosition() + CustomCollisionCenterOffset + (controller.height / 2 - CustomCollisionRadius) * Vector3.up;

    #region Custom Pathfinding
    public Vector3 Destination {  get; protected set; }
    private List<Vector3> path;
    #endregion

    public Entity Target { get; protected set; }

    private EnemySpawner spawner;

    [HideInInspector] public bool IsAttackAnimationPlaying;

    #region States
    public EnemyIdleState EnemyIdleState { get; protected set; }
    public EnemyChaseState EnemyChaseState { get; protected set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        EnemyIdleState = new EnemyIdleState(this);
        EnemyChaseState = new EnemyChaseState(this);
    }
    #endregion

    public void Init(EnemySpawner enemySpawner)
    {
        spawner = enemySpawner;
    }

    private protected override void OnAwake()
    {
        base.OnAwake();

        if(Ticker.Instance != null) Ticker.Instance.OnTick.AddListener(OnTick);
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

        SetStartState(EnemyIdleState);
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
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

        if (Ticker.Instance != null) Ticker.Instance.OnTick.RemoveListener(OnTick);
    }

    private protected override void OnOnDrawGizmos()
    {
        base.OnOnDrawGizmos();

        if(CustomCollisionRadius <= 0) return;

        Gizmos.color = Color.white;
        CustomGizmos.DrawWireCapsule(ChargeCollisionBottomPoint, CustomCollisionTopPoint, CustomCollisionRadius);
    }

    public override void Die()
    {
        base.Die();

        if (spawner != null) spawner.RemoveEnemyFromList(this);
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
    private List<Vector3> GetPathToDestination(Vector3 dest)
    {
        NavMeshPath path = new NavMeshPath();

        bool hasPath = NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);

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
        if (path == null) return;
        if (path.Count < 2) return;

        #region Debug
/*        Vector3 prevCorner = transform.position;
        foreach (Vector3 wayPoint in path)
        {
            Debug.DrawLine(prevCorner, wayPoint, Color.red);
            prevCorner = wayPoint;
        }*/
        #endregion

        Vector3 currDest = path[1];
        if (lookAtPath) LookAt(currDest);

        Vector3 dir = currDest - transform.position;
        dir.Normalize();

        UpdateHorizontalVelocity(dir);
        ApplyHorizontalVelocity();

        if (CloseToPoint(currDest, 0.05f))
        {
            path.RemoveAt(0);
        }
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
    /// Generates a random wander point within a specified radius range.
    /// Returns itself it cannot find a valid point after a certain number of iterations.
    /// Iteration count is set to 16 by default.
    /// </summary>
    /// <param name="wanderRadiusRange">The range of the wander radius.</param>
    /// <param name="iterationTryCount">The number of iterations to try finding a valid point.</param>
    /// <returns>The randomly generated wander point.</returns>
    public Vector3 GetRandomWanderPoint(Vector2 wanderRadiusRange, int iterationTryCount = 16)
    {
        // Raycast downwards to prevent charger from not wandering at all because it cannot reach
        RaycastHit raycastHit;
        for (int i = 0; i < iterationTryCount; i++)
        {
            float randomRadius = Random.Range(wanderRadiusRange.x, wanderRadiusRange.y);
            Vector3 randomPointOnUnitCircle = Random.onUnitSphere;
            randomPointOnUnitCircle.y = 0;
            Vector3 randomPoint = randomRadius * randomPointOnUnitCircle + transform.position;
            bool isValidPoint =
                Physics.Raycast(randomPoint + 100f * Vector3.up, Vector3.down, out raycastHit, Mathf.Infinity, LayerMask.GetMask("Ground"))
                && NavMesh.SamplePosition(raycastHit.point, out _, 0.5f, NavMesh.AllAreas);

            if (isValidPoint) return raycastHit.point;
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

        Collider[] hits = Physics.OverlapCapsule(ChargeCollisionBottomPoint, CustomCollisionTopPoint, CustomCollisionRadius, mask);
        if (hits == null) return result;
        if (hits.Length == 0) return result;

        foreach(Collider hit in hits)
        {
            if(IsOwnCollider(hit)) continue; // filter out own colliders

            result.Add(hit);
        }

        return isOrderedByAscendingDistance ? result.OrderBy(hit => Distance(hit.ClosestPoint(GetColliderCenterPosition() + CustomCollisionCenterOffset))).ToList() : result.ToList();
    }

    /// <summary>
    /// Checks if the enemy hit a wall.
    /// </summary>
    /// <param name="hit">The collider that was hit.</param>
    /// <returns>True if the enemy hit a wall, false otherwise.</returns>
    public bool DidHitWall(Collider hit)
    {
        return hit.gameObject.layer == LayerMask.NameToLayer("Ground");
    }

    /// <summary>
    /// Checks if the enemy hit a friendly entity.
    /// Hit must come from Damageable Colliders layer;
    /// </summary>
    /// <param name="hit">The collider that was hit.</param>
    /// <param name="entity">The friendly entity that was hit.</param>
    /// <returns>True if the enemy hit a friendly entity, false otherwise.</returns>
    public bool DidHitFriendlyEntity(Collider hit, out Entity entity)
    {
        entity = hit.GetComponentInParent<Entity>();

        if (entity == null) entity = hit.GetComponent<Entity>();
        if (entity.Team != Team) return false;

        return true;
    }

    /// <summary>
    /// Checks if the enemy hit an enemy entity.
    /// Hit must come from Damageable Colliders layer;
    /// </summary>
    /// <param name="hit">The collider that was hit.</param>
    /// <param name="entity">The enemy entity that was hit.</param>
    /// <returns>True if the enemy hit an enemy entity, false otherwise.</returns>
    public bool DidHitEnemyEntity(Collider hit, out Entity entity)
    {
        entity = hit.GetComponentInParent<Entity>();

        if (entity == null) return false;
        if (entity.Team == Team) return false;

        return true;
    }

    public void ClearTarget()
    {
        Target = null;
    }
}
