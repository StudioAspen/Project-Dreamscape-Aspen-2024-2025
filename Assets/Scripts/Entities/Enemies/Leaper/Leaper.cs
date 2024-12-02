using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.InteropServices;

public class Leaper : Enemy
{
    [field: Header("Leaper: Wander Settings")]
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public float WanderHopHeight { get; private set; } = 2f;

    [field: Header("Leaper: Attack Settings")]
    [field: SerializeField] public LayerMask LeapAttackLayerMask { get; private set; }
    [field: SerializeField] public float LeapAttackContactDamagePercent { get; private set; } = 100f;

    [field: Header("Leaper: Hop Settings")]
    [field: SerializeField] public int HopCount { get; private set; } = 2;
    [field: SerializeField] public float HopDistance { get; private set; } = 3f;
    [field: SerializeField] public float HopHeight { get; private set; } = .75f;

    #region States
    public LeaperAttackState LeaperAttackState { get; private set; }
    public LeaperHopState LeaperHopState { get; private set; }
    public LeaperWanderState LeaperWanderState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        LeaperAttackState = new LeaperAttackState(this);
        LeaperHopState = new LeaperHopState(this);
        LeaperWanderState = new LeaperWanderState(this);
        EnemyChaseState = new LeaperChaseState(this);
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
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
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
    }

    /// <summary>
    /// Checks for collisions with enemy entities and applies damage if a collision occurs.
    /// Pass in a reference to a list of hit entities to prevent multiple hits on the same entity.
    /// </summary>
    /// <param name="hitEntities">A reference to the list of hit entities.</param>
    public void CheckCollisions(ref List<Entity> hitEntities)
    {
        List<Collider> hits = GetCustomCollisionHits(LeapAttackLayerMask);

        foreach (Collider hit in hits)
        {
            if (DidHitEnemyEntity(hit, out Entity enemyEntity))
            {
                if (hitEntities.Contains(enemyEntity)) continue;
                hitEntities.Add(enemyEntity);

                enemyEntity.TakeDamage(CalculateDamage(LeapAttackContactDamagePercent), hit.ClosestPoint(GetColliderCenterPosition()), gameObject, true);
            }
        }
    }

    /// <summary>
    /// Calculates the velocity required to hop from the current position to an end position
    /// with a specified maximum hop height.
    /// </summary>
    /// <param name="endPosition">The target position to reach.</param>
    /// <param name="hopHeight">The maximum height of the hop.</param>
    /// <returns>The initial velocity vector needed to make the hop.</returns>
    public Vector3 CalculateHopVelocity(Vector3 endPosition, float hopHeight)
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

        // Calculate initial vertical velocity required to reach hopHeight
        float initialVerticalVelocity = Mathf.Sqrt(2 * gravity * hopHeight);

        // Total time of flight
        float timeToApex = initialVerticalVelocity / gravity; // Time to reach the peak
        float totalFlightTime = timeToApex + Mathf.Sqrt(2 * (hopHeight - verticalDisplacement) / gravity);

        // Calculate the horizontal velocity
        Vector3 horizontalVelocity = horizontalDisplacement / totalFlightTime;

        // Combine horizontal and vertical components into the final velocity vector
        Vector3 hopVelocity = horizontalVelocity + Vector3.up * initialVerticalVelocity;

        return hopVelocity;
    }

    /// <summary>
    /// Makes the enemy hop to a specified end position with a given hop height.
    /// </summary>
    /// <param name="endPosition">The target position to hop to.</param>
    /// <param name="hopHeight">The maximum height of the hop.</param>
    public void Hop(Vector3 endPosition, float hopHeight)
    {
        Vector3 hopVelocity = CalculateHopVelocity(endPosition, hopHeight);

        Launch(hopVelocity.normalized, hopVelocity.magnitude);
    }

    public void CreateTempHitVisual(Vector3 pos, float radius, Color color, float duration)
    {
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        temp.name = "TempHitVisual";
        temp.GetComponent<Collider>().enabled = false;
        temp.transform.localScale = radius * Vector3.one;
        temp.transform.position = pos;
        temp.GetComponent<Renderer>().material.color = color;
        Destroy(temp, duration);
    }
}