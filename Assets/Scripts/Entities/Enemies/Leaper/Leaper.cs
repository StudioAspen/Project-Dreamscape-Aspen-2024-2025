using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Leaper : Enemy
{
    [field: Header("Leaper: Cone Detection Settings")]
    [field: SerializeField] public float DetectionDistance { get; private set; } = 15f;
    [field: SerializeField] public float DetectionConeHalfAngle { get; private set; } = 40f;

    [field: Header("Leaper: Wander Settings")]
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public float WanderHopHeight { get; private set; } = 2f;

    [field: Header("Leaper: Chase Settings")]
    [field: SerializeField] public float ChaseHopHeight { get; private set; } = 1.25f;
    [field: SerializeField] public float ChaseHopDistance { get; private set; } = 2f;
    [field: SerializeField] public float StartReadyAttackDistance { get; private set; } = 2f;

    [field: Header("Leaper: Attack Settings")]
    [field: SerializeField] public LayerMask LeapAttackLayerMask { get; private set; }
    [field: SerializeField] public float AttackContactDamagePercent { get; private set; } = 150f;
    [field: SerializeField] public float RegularContactDamagePercent { get; private set; } = 100f;
    [field: SerializeField] public float AttackHopDuration { get; private set; } = .75f;

    [field: Header("Leaper: Ready Attack (Hop) Settings")]
    [field: SerializeField] public int ReadyAttackHopCount { get; private set; } = 2;
    [field: SerializeField] public float ReadyAttackHopDistance { get; private set; } = 3f;
    [field: SerializeField] public float ReadyAttackHopHeight { get; private set; } = .75f;
    [field: SerializeField] public float ReadyAttackStartDelay { get; private set; } = 0.75f;

    #region States
    public LeaperWanderState LeaperWanderState { get; private set; }
    public LeaperChaseState LeaperChaseState { get; private set; }
    public LeaperReadyAttackState LeaperReadyAttackState { get; private set; }
    public LeaperAttackState LeaperAttackState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        LeaperWanderState = new LeaperWanderState(this);
        EnemyChaseState = new LeaperChaseState(this);
        LeaperReadyAttackState = new LeaperReadyAttackState(this);
        LeaperAttackState = new LeaperAttackState(this);
        LeaperChaseState = EnemyChaseState as LeaperChaseState;
    }
    #endregion

    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

        SetStartState(LeaperWanderState);

        OnGrounded += Leaper_OnGrounded;
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();

        OnGrounded -= Leaper_OnGrounded;
    }

    private protected override void OnStart()
    {
        base.OnStart();

        SetDefaultState(LeaperWanderState);  
    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    private protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    private protected override void OnOnDrawGizmos()
    {
        base.OnOnDrawGizmos();

#if UNITY_EDITOR
        Gizmos.color = Color.red;
        CustomDebug.DrawWireCircle(transform.position, targetDetectionRadius);
        CustomDebug.DrawWireCone(CustomCollisionTopPoint, transform.forward, DetectionConeHalfAngle, DetectionDistance);
#endif
    }

    public override void TryAssignTarget()
    {
        // replace default radius-based target assignment with cone-based target assignment
        TryAssignTargetWithCone(DetectionDistance, DetectionConeHalfAngle);
    }

    /// <summary>
    /// Checks for collisions with enemy entities and applies damage if a collision occurs.
    /// Pass in a reference to a list of hit entities to prevent multiple hits on the same entity.
    /// </summary>
    /// <param name="hitEntities">A reference to the list of hit entities.</param>
    public void CheckCollisions(float damagePercent, ref List<Entity> hitEntities)
    {
        List<Collider> hits = GetCustomCollisionHits(LeapAttackLayerMask);

        foreach (Collider hit in hits)
        {
            if (DidHitEnemyEntity(hit, out Entity enemyEntity))
            {
                if (hitEntities.Contains(enemyEntity)) continue;
                hitEntities.Add(enemyEntity);

                enemyEntity.TakeDamage(CalculateDamage(damagePercent), hit.ClosestPoint(GetColliderCenterPosition()), gameObject, true);

                //switch to some state
            }
        }
    }

    /// <summary>
    /// Calculates the velocity required to hop from the current position to an end position with a specified maximum hop height.
    /// Default hop duration is calculated based on the height difference between the current and end positions, unless specified.
    /// If a hop duration is specified, the hop height is ignored.
    /// </summary>
    /// <param name="endPosition">The target position to reach.</param>
    /// <param name="hopHeight">The maximum height of the hop.</param>
    /// <param name="hopDuration">The duration of the hop.</param>
    /// <returns>The initial velocity vector needed to make the hop.</returns>
    private Vector3 CalculateHopVelocity(Vector3 endPosition, float hopHeight, float hopDuration = 0f)
    {
        // Gravity constant (positive value, assuming downward acceleration)
        float gravity = Mathf.Abs(Physics.gravity.y);

        // Current position of the entity
        Vector3 startPosition = transform.position;

        // Horizontal displacement (ignoring vertical component)
        Vector3 horizontalDisplacement = new Vector3(
            endPosition.x - startPosition.x,
            0,
            endPosition.z - startPosition.z
        );

        float horizontalDistance = horizontalDisplacement.magnitude;

        // Vertical displacement (difference in height)
        float verticalDisplacement = endPosition.y - startPosition.y;

        float totalFlightTime;

        float initialVerticalVelocity;

        if (hopDuration == 0f)
        {
            // Calculate initial vertical velocity required to reach hopHeight
            initialVerticalVelocity = Mathf.Sqrt(2 * gravity * hopHeight);

            // Total time of flight
            float timeToApex = initialVerticalVelocity / gravity; // Time to reach the peak
            totalFlightTime = timeToApex + Mathf.Sqrt(2 * (hopHeight - verticalDisplacement) / gravity);
        }
        else
        {
            // Use the provided hopDuration to calculate vertical velocity
            totalFlightTime = hopDuration;

            // Using kinematic equation to solve for initial vertical velocity
            // s = v*t - 0.5*g*t^2
            // verticalDisplacement = v * totalFlightTime - 0.5 * gravity * totalFlightTime^2
            initialVerticalVelocity = (verticalDisplacement + 0.5f * gravity * Mathf.Pow(totalFlightTime, 2)) / totalFlightTime;
        }

        // Calculate the horizontal velocity
        Vector3 horizontalVelocity = horizontalDisplacement / totalFlightTime;

        // Combine horizontal and vertical components into the final velocity vector
        Vector3 hopVelocity = horizontalVelocity + Vector3.up * initialVerticalVelocity;

        return hopVelocity;
    }

    /// <summary>
    /// Makes the enemy hop to a specified end position with a given hop height.
    /// Default hop duration is calculated based on the height difference between the current and end positions, unless specified.
    /// </summary>
    /// <param name="endPosition">The target position to hop to.</param>
    /// <param name="hopHeight">The maximum height of the hop.</param>
    public void Hop(Vector3 endPosition, float hopHeight, float hopDuration = 0f)
    {
        Vector3 hopVelocity = CalculateHopVelocity(endPosition, hopHeight, hopDuration);

        Launch(hopVelocity.normalized, hopVelocity.magnitude);
    }

    private void Leaper_OnGrounded(Vector3 groundedPosition)
    {
        if (CurrentState == EntityDeathState) return;

        TransitionToAnimation("FlatMovement");
    }
}