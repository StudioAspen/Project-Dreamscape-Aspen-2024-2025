using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    [field: Header("Slime: Detection Settings")]
    [field: SerializeField] public float DetectionDistance { get; private set; } = 15f;
    [field: SerializeField] public float DetectionConeHalfAngle { get; private set; } = 40f;

    [field: SerializeField] public bool isSplit { get; private set; } = false; 
    

    #region States
    public SlimeIdleState SlimeIdleState {get; private set;}
    public SlimePlayerDetectedState SlimePlayerDetectedState {get; private set;}
    public SlimeAttackState SlimeAttackState {get; private set;}
    public SlimeSplitState SlimeSplitState {get; private set;}
    public SlimeGrowthState SlimeGrowthState {get; private set;}

    // private void Init(Entity entity)
    // {
    //     slime = entity as Slime;
    // }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        SlimeIdleState = EntityBaseState.InitializeOrCreate<SlimeIdleState>(this);
        SlimePlayerDetectedState = EntityBaseState.InitializeOrCreate<SlimePlayerDetectedState>(this);
        SlimeAttackState = EntityBaseState.InitializeOrCreate<SlimeAttackState>(this);
        SlimeSplitState = EntityBaseState.InitializeOrCreate<SlimeSplitState>(this);
        SlimeGrowthState = EntityBaseState.InitializeOrCreate<SlimeGrowthState>(this);

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

    }

    private protected override void OnStart()
    {
        base.OnStart();

        SetDefaultState(SlimeSplitState);  

        Debug.Log(Spawner);
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
}
