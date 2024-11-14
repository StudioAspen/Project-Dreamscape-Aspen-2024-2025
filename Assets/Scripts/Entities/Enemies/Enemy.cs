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
    [field: Header("Enemy: References")]
    [SerializeField, Self] protected CapsuleCollider capsuleCollider;
    [SerializeField, Child] private TMP_Text debugStateText;
    [field: SerializeField, Self] public Rigidbody RigidBody { get; private set; }

    [field : Header("Enemy: Settings")]
    [field: SerializeField] public int Cost { get; protected set; }
    public float MovementSpeed => SpeedModifier * baseSpeed;
    private float totalSpeedModifierForAnimation;

    public Vector3 Destination {  get; protected set; }
    private List<Vector3> path;
    private bool lookAtPath;

    public Entity Target { get; protected set; }
    private EnemySpawner spawner;

    [HideInInspector] public bool IsAttackAnimationPlaying;
    [HideInInspector] public bool UseRootMotion;

    #region States
    public EnemyIdleState EnemyIdleState { get; protected set; }
    public EnemyChaseState EnemyChaseState { get; protected set; }
    #endregion

    public void Init(EnemySpawner e)
    {
        spawner = e;
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        if(Ticker.Instance != null) Ticker.Instance.OnTick.AddListener(OnTick);
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();

        SetStartState(EnemyIdleState);
    }

    protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    protected override void OnStart()
    {
        base.OnStart();

        ChangeTeam(1);

        SetDefaultState(EnemyIdleState);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        HandleAnimations();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        
        MoveTowardsDestination();
    }

    private void LateUpdate()
    {
        DebugState();
    }

    private void OnAnimatorMove()
    {
        OnOnAnimatorMove();
    }

    protected virtual void OnOnAnimatorMove()
    {
        if (!UseRootMotion) return;

        float modelScale = model.localScale.x;
        Vector3 desiredAnimationMovement = modelScale * animator.deltaPosition;
        //desiredAnimationMovement.y = 0f;

        RigidBody.MovePosition(transform.position + desiredAnimationMovement);
    }

    protected virtual void OnTick()
    {
        TryAssignTarget();
    }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        EnemyIdleState = new EnemyIdleState(this);
        EnemyChaseState = new EnemyChaseState(this);
    }

    protected override void CheckGrounded()
    {
        IsGrounded = Physics.CheckSphere(transform.position + 9f * capsuleCollider.radius / 10f * Vector3.up, capsuleCollider.radius, physicsSettings.GroundLayer);
    }

    private void DebugState()
    {
        debugStateText.transform.parent.rotation = Quaternion.LookRotation(debugStateText.transform.parent.position - Camera.main.transform.position);

        debugStateText.text = $"{CurrentState.GetType()}";
    }

    private void HandleAnimations()
    {
        totalSpeedModifierForAnimation = Mathf.Lerp(totalSpeedModifierForAnimation, SpeedModifier, 5f * Time.deltaTime);

        animator.SetFloat("MovementSpeed", totalSpeedModifierForAnimation);
    }

    private List<Vector3> GetPathToDestination(Vector3 dest)
    {
        NavMeshPath path = new NavMeshPath();

        bool hasPath = NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);

        if (!hasPath) return null;
        if(path.corners.Length == 0) return null;

        return path.corners.ToList();
    }

    private void MoveTowardsDestination()
    {
        if (path == null) return;
        if(path.Count < 2) return;

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

        Move(dir);

        if (Distance(currDest) < 0.05f)
        {
            path.RemoveAt(0);
        }
    }

    public void CancelPath()
    {
        path = null;
    }

    public void Move(Vector3 dir)
    {
        RigidBody.MovePosition(transform.position + MovementSpeed * Time.deltaTime * dir.normalized);
    }

    public void SetDestination(Vector3 dest, bool lookAtPath)
    {
        Destination = dest;
        path = GetPathToDestination(dest);
        this.lookAtPath = lookAtPath;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        if (Ticker.Instance != null) Ticker.Instance.OnTick.RemoveListener(OnTick);
    }

    public override void Die()
    {
        base.Die();
        spawner.RemoveEnemyFromList(this);
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

    public override void LookAt(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);

        RigidBody.MoveRotation(Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
    }

    public override void Fling(Vector3 direction, float force, float stunDuration)
    {
        RigidBody.AddForce(force * direction.normalized, ForceMode.Impulse);
    }
}
