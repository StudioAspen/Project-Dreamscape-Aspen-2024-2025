using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // custom pathfinding
    public Vector3 Destination {  get; protected set; }
    private List<Vector3> path;
    private bool lookAtPath;

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
    /// </summary>
    public void MoveTowardsDestination()
    {
        if (path == null) return;
        if (path.Count < 2) return;

        #region Debug
        Vector3 prevCorner = transform.position;
        foreach (Vector3 wayPoint in path)
        {
            Debug.DrawLine(prevCorner, wayPoint, Color.red);
            prevCorner = wayPoint;
        }
        #endregion

        Vector3 currDest = path[1];
        if (lookAtPath) LookAt(currDest);

        Vector3 dir = currDest - transform.position;
        dir.Normalize();

        UpdateGroundedVelocity(dir);
        GroundedMove();

        if (Distance(currDest) < 0.05f)
        {
            path.RemoveAt(0);
        }
    }

    public void CancelPath()
    {
        path = null;
    }

    public void SetDestination(Vector3 dest, bool lookAtPath)
    {
        Destination = dest;
        path = GetPathToDestination(dest);
        this.lookAtPath = lookAtPath;
    }

    private protected override void OnDeath()
    {
        base.OnDeath();
        if (Ticker.Instance != null) Ticker.Instance.OnTick.RemoveListener(OnTick);
    }

    public override void Die()
    {
        base.Die();

        if(spawner != null) spawner.RemoveEnemyFromList(this);
    }

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

    public void ClearTarget()
    {
        Target = null;
    }
}
