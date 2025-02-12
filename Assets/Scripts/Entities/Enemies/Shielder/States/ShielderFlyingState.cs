using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShielderFlyingState : EntityLaunchState
{
    [field: SerializeField] public LayerMask ShielderFlyingLayerMask { get; private set; }  
    private List<Entity> entitiesHitByCurrentLeap = new List<Entity>();

    [field: SerializeField] public float AttackContactDamagePercent { get; private set; } = 500f;


    private Shielder shielder;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);
        shielder = entity as Shielder;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        entitiesHitByCurrentLeap.Clear();
    }

    public override void OnUpdate()
    {
        Debug.Log("Collision Detectable");
        base.OnUpdate();

        
        if (shielder.IsGrounded)
        {
            shielder.ChangeState(shielder.ShielderIdleState);
            return;
        }
        
           
        shielder.ApplyGravity();
        shielder.ApplyHorizontalVelocity();

        shielder.CheckCollisions(AttackContactDamagePercent, ref entitiesHitByCurrentLeap);
    }

    
}

