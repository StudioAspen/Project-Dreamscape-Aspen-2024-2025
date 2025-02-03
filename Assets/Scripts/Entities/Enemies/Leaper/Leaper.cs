using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Leaper : Enemy
{
    [field: Header("Leaper: Cone Detection Settings")]
    [field: SerializeField] public float DetectionDistance { get; private set; } = 15f;
    [field: SerializeField] public float DetectionConeHalfAngle { get; private set; } = 40f;

    #region States
    public LeaperWanderState LeaperWanderState { get; private set; }
    public LeaperChaseState LeaperChaseState { get; private set; }
    public LeaperReadyAttackState LeaperReadyAttackState { get; private set; }
    public LeaperAttackState LeaperAttackState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        LeaperWanderState = EntityBaseState.InitializeOrCreate<LeaperWanderState>(this);
        LeaperReadyAttackState = EntityBaseState.InitializeOrCreate<LeaperReadyAttackState>(this);
        LeaperAttackState = EntityBaseState.InitializeOrCreate<LeaperAttackState>(this);
        EnemyChaseState = EntityBaseState.InitializeOrCreate<LeaperChaseState>(this);
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
    /// <param name="damagePercent">The percentage damage to apply.</param>
    /// <param name="hitEntities">A reference to the list of hit entities.</param>
    public void CheckCollisions(float damagePercent, ref List<Entity> hitEntities)
    {
        List<Collider> hits = GetCustomCollisionHits(LeaperAttackState.LeapAttackLayerMask);

        foreach (Collider hit in hits)
        {
            if (DidHitEnemyEntity(hit, out Entity enemyEntity))
            {
                if (hitEntities.Contains(enemyEntity)) continue;
                hitEntities.Add(enemyEntity);

                DealDamageToOtherEntity(enemyEntity, CalculateDamage(damagePercent), hit.ClosestPoint(GetColliderCenterPosition()));
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
        float gravity = Mathf.Abs(PhysicsConfig.Gravity);

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
            initialVerticalVelocity = Mathf.Sqrt(2 * gravity * Mathf.Abs(hopHeight));

            totalFlightTime = CalculateHopDuration(endPosition, hopHeight);
        }
        else
        {
            // Use the provided hopDuration to calculate vertical velocity
            totalFlightTime = hopDuration;

            // Using kinematic equation to solve for initial vertical velocity
            // s = v*t - 0.5*g*t^2
            // verticalDisplacement = v * totalFlightTime - 0.5 * gravity * totalFlightTime^2
            initialVerticalVelocity = (verticalDisplacement + 0.5f * gravity * Mathf.Pow(totalFlightTime, 2)) / totalFlightTime;
            if(float.IsNaN(initialVerticalVelocity)) initialVerticalVelocity = 0f;
        }

        // Calculate the horizontal velocity
        Vector3 horizontalVelocity = horizontalDisplacement / totalFlightTime;
        if(totalFlightTime == 0f) horizontalVelocity = Vector3.zero;

        // Combine horizontal and vertical components into the final velocity vector
        Vector3 hopVelocity = horizontalVelocity + Vector3.up * initialVerticalVelocity;

        return hopVelocity;
    }

    /// <summary>
    /// Calculates the total duration required to hop from the current position to the end position
    /// with a specified maximum hop height.
    /// </summary>
    /// <param name="endPosition">The target position to reach.</param>
    /// <param name="hopHeight">The maximum height of the hop.</param>
    /// <returns>The total duration of the hop.</returns>
    public float CalculateHopDuration(Vector3 endPosition, float hopHeight)
    {
        // Gravity constant (positive value, assuming downward acceleration)
        float gravity = Mathf.Abs(PhysicsConfig.Gravity);

        // Current position of the entity
        Vector3 startPosition = transform.position;

        // Vertical displacement (difference in height)
        float verticalDisplacement = endPosition.y - startPosition.y;

        // Calculate initial vertical velocity required to reach hopHeight
        float initialVerticalVelocity = Mathf.Sqrt(2 * gravity * Mathf.Abs(hopHeight));

        // Time to reach the apex of the hop
        float timeToApex = initialVerticalVelocity / gravity;

        // Time to descend from the apex to the end position
        float timeToDescend = Mathf.Sqrt(2 * (hopHeight - verticalDisplacement) / gravity);
        if(float.IsNaN(timeToDescend)) timeToDescend = 0f;

        // Total flight time (ascent + descent)
        float totalFlightTime = timeToApex + timeToDescend;

        return totalFlightTime;
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
        
        if(hopVelocity == Vector3.zero) return;

        Launch(hopVelocity.normalized, hopVelocity.magnitude);
    }

    private void Leaper_OnGrounded(Vector3 groundedPosition)
    {
        if (CurrentState == EntityDeathState) return;
        if (CurrentState == EntitySpawnState) return;
        if (CurrentState == EntityLaunchState) return;
        if (CurrentState == EntityStaggeredState) return;

        TransitionToAnimation("FlatMovement");
    }
}