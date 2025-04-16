using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Slime : Enemy
{
    [field: Header("Slime: Detection Settings")]
    [field: SerializeField] public float DetectionDistance { get; private set; } = 15f;
    [field: SerializeField] public float DetectionConeHalfAngle { get; private set; } = 40f;

    [field: Header("Slime: Split Settings")]
    [field: SerializeField] public int SplitCount { get; private set; } = 2;
    [field: SerializeField] public float SmallSize { get; private set; } = 0.5f;
    [field: SerializeField] public float SmallMaxHealth { get; private set; } = 0.5f;
    [field: SerializeField] public int OriginalBaseMaxHealth { get; private set; } = 50;
    [field: SerializeField] public float SmallDamageModifier { get; private set; } = 0.5f;
    public bool IsSmall { get; private set; } = false;
    public bool IsClone { get; private set; } = false;
    private float timeSinceLastDamage;

    [field: Header("Slime: Animation")]
    [field: SerializeField] public AnimationClip JumpAnimationClip { get; private set; }

    private Enemy slimeEnemyPrefab;

    #region States
    [field: Header("Slime: States")]
    [field: SerializeField] public SlimeWanderState SlimeWanderState {get; private set;}
    [field: SerializeField] public SlimeChaseState SlimeChaseState  {get; private set;}
    [field: SerializeField] public SlimeAttackExpandState SlimeAttackExpandState {get; private set;}
    [field: SerializeField] public SlimeAttackShrinkState SlimeAttackShrinkState { get; private set;}
    [field: SerializeField] public SlimeGrowthState SlimeGrowthState {get; private set;}
    [field: SerializeField] public SlimeDeathState SlimeDeathState { get; private set;}

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        SlimeChaseState.Init(this);
        SlimeWanderState.Init(this);
        SlimeAttackExpandState.Init(this);
        SlimeAttackShrinkState.Init(this);
        SlimeGrowthState.Init(this);
        SlimeDeathState.Init(this);

        EnemyChaseState = SlimeChaseState; // In case you accidentally change to EnemyChaseState
        EntityDeathState = SlimeDeathState;
    }
    #endregion    

    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

        SetSmall(IsSmall);

        OnEntityTakeDamage += Entity_OnEntityTakeDamage;
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();

        OnEntityTakeDamage -= Entity_OnEntityTakeDamage;

        SetSmall(false);
    }

    private protected override void OnStart()
    {
        base.OnStart();
    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();

        HandleGrowthConditionWhenSmall();
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

    private protected override void OnDeath()
    {
        base.OnDeath();
    }

    private void Entity_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        timeSinceLastDamage = 0f;
    }

    // <summary>
    // Checks for collisions with enemy entities and applies damage if a collision occurs.
    // Pass in a reference to a list of hit entities to prevent multiple hits on the same entity.
    // </summary>
    // <param name="damagePercent">The percentage damage to apply.</param>
    // <param name="hitEntities">A reference to the list of hit entities.</param>
    public void CheckCollisions(float damageMultiplier, ref List<Entity> hitEntities)
    {
        List<Collider> hits = GetCustomCollisionHits(SlimeAttackExpandState.SlimeAttackLayerMask);
                
        foreach (Collider hit in hits)
        {
            if (DidHitEnemyEntity(hit, out Entity enemyEntity))
            {
                if (hitEntities.Contains(enemyEntity)) continue;
                hitEntities.Add(enemyEntity);

                DealDamageToOtherEntity(enemyEntity, CalculateDamage(damageMultiplier), hit.ClosestPoint(GetColliderCenterPosition()));
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
            if (float.IsNaN(initialVerticalVelocity)) initialVerticalVelocity = 0f;
        }

        // Calculate the horizontal velocity
        Vector3 horizontalVelocity = horizontalDisplacement / totalFlightTime;
        if (totalFlightTime == 0f) horizontalVelocity = Vector3.zero;

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
        if (float.IsNaN(timeToDescend)) timeToDescend = 0f;

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

        if (hopVelocity == Vector3.zero) return;

        Launch(hopVelocity.normalized, hopVelocity.magnitude);
    }

    private void Slime_OnGrounded(Vector3 groundedPosition)
    {
        if (CurrentState == EntityDeathState) return;
        if (CurrentState == EntitySpawnState) return;
        if (CurrentState == EntityLaunchState) return;
        if (CurrentState == EntityStunnedState) return;
        if (CurrentState == EntityStaggeredState) return;

        PlayDefaultAnimation();
    }

    public override bool CanBeStaggered()
    {
        bool cannotBeStaggered = 
            CurrentState == SlimeGrowthState ||
            CurrentState == SlimeAttackExpandState ||
            CurrentState == SlimeAttackShrinkState ||
            CurrentState == EntityStunnedState;

        return !cannotBeStaggered;
    }

    /// <summary>
    /// Makes the slime small by adjusting the maxhealth and size.
    /// </summary>
    /// <param name="isSmall">Whether to make the slime small or big.</param>
    public void SetSmall(bool isSmall)
    {
        IsSmall = isSmall;

        MaxHealth.SetBaseValue(IsSmall ? OriginalBaseMaxHealth * SmallMaxHealth : OriginalBaseMaxHealth);
        SizeScale.SetBaseValue(IsSmall ? SmallSize : 1f);
        DamageModifier.SetBaseValue(IsSmall ? SmallDamageModifier : 1f);
    }

    /// <summary>
    /// Updates the IsClone flag to true.
    /// </summary>
    public void UpdateCloneFlag()
    {
        IsClone = true;
    }

    /// <summary>
    /// Keeps track of how long the small slime has went without taken damage to grow
    /// </summary>
    private void HandleGrowthConditionWhenSmall()
    {
        if (!IsSmall)
        {
            timeSinceLastDamage = 0f;
            return;
        }

        if(timeSinceLastDamage > SlimeGrowthState.NoDamageTakenTargetDuration)
        {
            ChangeState(SlimeGrowthState);
        }
        else
        {
            timeSinceLastDamage += LocalDeltaTime;
        }
    }

    /// <summary>
    /// Used to check if an entity is a slime and small.
    /// </summary>
    /// <param name="entity">The entity to check</param>
    /// <returns></returns>
    public static bool IsEntityASmallSlime(Entity entity)
    {
        Slime slime = entity as Slime;
        if(slime == null) return false;
        return slime.IsSmall;
    }

    /// <summary>
    /// Used to check if an entity is a cloned slime.
    /// </summary>
    /// <param name="entity">The entity to check</param>
    /// <returns></returns>
    public static bool IsEntityACloneSlime(Entity entity)
    {
        Slime slime = entity as Slime;
        if (slime == null) return false;
        return slime.IsClone;
    }
}
