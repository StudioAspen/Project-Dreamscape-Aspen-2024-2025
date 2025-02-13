using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    #region States
    public SlimeIdleState SlimeIdleState {get; private set;}

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        SlimeIdleState = EntityBaseState.InitializeOrCreate<SlimeIdleState>(this);

    }

    #endregion    

    private protected override void OnAwake()
    {
        base.OnAwake();
        
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
        OnEntityDeath -= Entity_OnEntityDeath;
    }

    private protected override void OnStart()
    {
        base.OnStart();
        Debug.Log(isSplit);
        slimeEnemyPrefab = GetEnemyPrefabFromCurrentType();
        OnEntityDeath += Entity_OnEntityDeath;
        SetDefaultState(SlimeIdleState);
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

    private protected override void OnDeath()
    {
        base.OnDeath();
    }

    private void Entity_OnEntityDeath(GameObject entityGameObject)
    {
        // Debug.Log("pimp down, pimp in distress");
        onDuplicate();
        OnEntityDeath -= Entity_OnEntityDeath;
    }

    // <summary>
    // Checks for collisions with enemy entities and applies damage if a collision occurs.
    // Pass in a reference to a list of hit entities to prevent multiple hits on the same entity.
    // </summary>
    // <param name="damagePercent">The percentage damage to apply.</param>
    // <param name="hitEntities">A reference to the list of hit entities.</param>
    // public void CheckCollisions(float damagePercent, ref List<Entity> hitEntities)
    // {
    //     // List<Collider> hits = GetCustomCollisionHits(LeaperAttackState.LeapAttackLayerMask);

    //     foreach (Collider hit in hits)
    //     {
    //         if (DidHitEnemyEntity(hit, out Entity enemyEntity))
    //         {
    //             if (hitEntities.Contains(enemyEntity)) continue;
    //             hitEntities.Add(enemyEntity);

    //             DealDamageToOtherEntity(enemyEntity, CalculateDamage(damagePercent), hit.ClosestPoint(GetColliderCenterPosition()));
    //         }
    //     }
    // }




    private Enemy GetEnemyPrefabFromCurrentType()
    {
        if(this.Spawner == null)
        {
            Debug.Log("spawner = null ");
        }
        
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

    // method for duplicate but the code in it should work in Entity_OnEntityDeath without it
    // only here for debugging 
    private void onDuplicate()
    {
            
            if(isSplit == true)
            {
                Debug.Log("cancel");
                return;
            }
            
            for (int i = 0; i < SplitCount; i++ )
            {
                // if you suspect this is crashing game uncomment bellow
                // Debug.Break();
                float angle = i * (360f / SplitCount);

            
                Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad)) * 1f;
            
            
                Vector3 spawnPos = this.transform.position + offset;
                // this.Spawner.SpawnEnemy(slimeEnemyPrefab, spawnPos);

                // similar to previous code except this time it changes isSplit to true
                // so the duplicate cant split
                Slime duplicateSlime = this.Spawner.SpawnEnemy(slimeEnemyPrefab, spawnPos) as Slime;
                if (isSplit == false)
                {
                    duplicateSlime.isSplit = true;
                    
                }

        
            }
    }
}
