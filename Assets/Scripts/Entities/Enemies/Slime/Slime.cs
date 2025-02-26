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
    public bool IsSmall { get; private set; } = false;
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

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        SlimeChaseState.Init(this);
        SlimeWanderState.Init(this);
        SlimeAttackExpandState.Init(this);
        SlimeAttackShrinkState.Init(this);
        SlimeGrowthState.Init(this);

        EnemyChaseState = SlimeChaseState; // In case you accidentally change to EnemyChaseState
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

        OnEntityDestroyed += Entity_OnEntityDestroyed;
        OnEntityTakeDamage += Entity_OnEntityTakeDamage;
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
        OnEntityDestroyed -= Entity_OnEntityDestroyed;
        OnEntityTakeDamage -= Entity_OnEntityTakeDamage;
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

    private void Entity_OnEntityDestroyed(Entity entityDestroyed, GameObject source)
    {
        // small slimes dont split, they die
        if(IsSmall) return;

        slimeEnemyPrefab = GetEnemyPrefabFromCurrentType();
        if (slimeEnemyPrefab == null) return;

        for (int i = 0; i < SplitCount; i++ )
        {
            // if you suspect this is crashing game uncomment bellow
            // Debug.Break();
            float angle = i * (360f / SplitCount);
            
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));
        
            Vector3 spawnPos = transform.position + offset;

            Slime duplicateSlime = Spawner.SpawnEnemy(slimeEnemyPrefab, spawnPos) as Slime;
            duplicateSlime.SetSmall(true); 
        }

        OnEntityDestroyed -= Entity_OnEntityDestroyed;
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

    // nothing changed from the duplicate elite variant script
    private Enemy GetEnemyPrefabFromCurrentType()
    {
        foreach (Enemy enemyPrefab in Spawner.NeutralEnemyPrefabs)
        {
            if (enemyPrefab.GetType() == GetType())
            {
                return enemyPrefab;
            }
        }

        Debug.LogWarning("Could not find enemy prefab from current type.");

        return null;
    }

    private Vector3 CalculateHopVelocity(Vector3 endPosition, float hopHeight, float hopDuration = 0f)
    {
        float gravity = Mathf.Abs(PhysicsConfig.Gravity);

        Vector3 startPosition = transform.position;

        Vector3 horizontalDisplacement = new Vector3 (
            endPosition.x - startPosition.x,
            0,
            endPosition.y - startPosition.y
        );

        float horizontalDistance = horizontalDisplacement.magnitude;

        float verticalDisplacement = endPosition.y - startPosition.y;

        float totalFlightTime;

        float initialVerticalVelocity;

        if (hopDuration == 0f)
        {
            initialVerticalVelocity = Mathf.Sqrt(2 * gravity * Mathf.Abs(hopHeight));

            totalFlightTime = CalculateHopDuration(endPosition, hopHeight);
        }
        else
        {
            totalFlightTime = hopDuration;

            initialVerticalVelocity = (verticalDisplacement + 0.5f * gravity * Mathf.Pow(totalFlightTime, 2)) / totalFlightTime;
            if(float.IsNaN(initialVerticalVelocity)) initialVerticalVelocity = 0f;            
        }

        Vector3 horizontalVelocity = horizontalDisplacement /totalFlightTime;
        if(totalFlightTime == 0f) horizontalVelocity = Vector3.zero;

        Vector3 hopVelocity = horizontalVelocity + Vector3.up * initialVerticalVelocity;
        
        return hopVelocity;
    }

    public float CalculateHopDuration(Vector3 endPosition, float hopHeight)
    {
        float gravity = Mathf.Abs(PhysicsConfig.Gravity);

        Vector3 startPosition = transform.position;

        float verticalDisplacement = endPosition.y - startPosition.y;
        float initialVerticalVelocity = Mathf.Sqrt(2* gravity * Mathf.Abs(hopHeight));
        float timeToApex = initialVerticalVelocity / gravity;
        float timeToDescend = Mathf.Sqrt(2* (hopHeight - verticalDisplacement) / gravity);

        if(float.IsNaN(timeToDescend)) timeToDescend = 0f;

        float totalFlightTime = timeToApex + timeToDescend;

        return totalFlightTime;
    }

    public void Hop(Vector3 endPosition, float hopHeight, float hopDuration = 0f)
    {
        Vector3 hopVelocity = CalculateHopVelocity(endPosition, hopHeight, hopDuration);

        if(hopVelocity== Vector3.zero) return;

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
}
