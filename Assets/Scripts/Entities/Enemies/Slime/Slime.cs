using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Slime : Enemy
{
    [field: Header("Slime: Detection Settings")]
    [field: SerializeField] public float DetectionDistance { get; private set; } = 15f;
    [field: SerializeField] public float DetectionConeHalfAngle { get; private set; } = 40f;

    // if true is small else false
    // [field: SerializeField] public bool isSplit { get; private set; }
    [field: SerializeField] public int SplitCount { get; private set; } = 2;
    
    
    
    private Enemy slimeEnemyPrefab;
    public bool isSplit = false;
    public bool hasAttacked = false;
    public float startScale = 0;



    #region States
    public SlimeWanderState SlimeWanderState {get; private set;}
    public SlimeChaseState SlimeChaseState  {get; private set;}
    public SlimeAttackState SlimeAttackState {get; private set;}
    public SlimeGrowthState SlimeGrowthState {get; private set;}

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        SlimeAttackState = EntityBaseState.InitializeOrCreate<SlimeAttackState>(this);
        SlimeGrowthState = EntityBaseState.InitializeOrCreate<SlimeGrowthState>(this);

        SlimeWanderState = EntityBaseState.InitializeOrCreate<SlimeWanderState>(this);
        EnemyChaseState = EntityBaseState.InitializeOrCreate<SlimeChaseState>(this);
        SlimeChaseState = EnemyChaseState as SlimeChaseState;


    }

    #endregion    

    private protected override void OnAwake()
    {
        base.OnAwake();
        
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();
        OnEntityDeath += Entity_OnEntityDeath;
        if(isSplit)
        {
            this.MaxHealth = 20;
            startScale = 1f;
            SetDefaultState(SlimeGrowthState);
        }
        else
        {
            this.MaxHealth = 40;
            startScale = transform.localScale.x;
            SetDefaultState(SlimeWanderState);
        }

    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
        OnEntityDeath -= Entity_OnEntityDeath;
    }

    private protected override void OnStart()
    {
        base.OnStart();

    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();

        if (CurrentState == EntityLaunchState) 
        {
            this.CurrentHealth = 0;
        }
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

    private protected override void OnDeath()
    {
        base.OnDeath();
    }

    private void Entity_OnEntityDeath(GameObject entityGameObject)
    {
        
        slimeEnemyPrefab = GetEnemyPrefabFromCurrentType();
        
        // returns without doing anything if already "died"
       if(isSplit == true)
        {
            return;
        }
            
        for (int i = 0; i < SplitCount; i++ )
        {
            // if you suspect this is crashing game uncomment bellow
            // Debug.Break();
            float angle = i * (360f / SplitCount);
            
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad)) * 1f;
        
            Vector3 spawnPos = this.transform.position + offset;

            // similar to previous code except this time it changes isSplit to true
            // so the duplicate cant split once it has already done so 
            Slime duplicateSlime = this.Spawner.SpawnEnemy(slimeEnemyPrefab, spawnPos) as Slime;
            if (isSplit == false)
            {
                duplicateSlime.isSplit = true;
                duplicateSlime.ChangeState(duplicateSlime.SlimeGrowthState);
                duplicateSlime.transform.localScale = Vector3.one * 0.75f;
            }

    
        }
        OnEntityDeath -= Entity_OnEntityDeath;
    }

    // <summary>
    // Checks for collisions with enemy entities and applies damage if a collision occurs.
    // Pass in a reference to a list of hit entities to prevent multiple hits on the same entity.
    // </summary>
    // <param name="damagePercent">The percentage damage to apply.</param>
    // <param name="hitEntities">A reference to the list of hit entities.</param>
    public void CheckCollisions(float damageMultiplier, ref List<Entity> hitEntities)
    {

        List<Collider> hits = GetCustomCollisionHits(SlimeAttackState.SlimeAttackLayerMask);
                
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
        foreach (Enemy enemyPrefab in this.Spawner.NeutralEnemyPrefabs)
        {
            if (enemyPrefab.GetType() == this.GetType())
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
        if (CurrentState == EntityStaggeredState) return;

        TransitionToAnimation("FlatMovement");
    }
    
}
