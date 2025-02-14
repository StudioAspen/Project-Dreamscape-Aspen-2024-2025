using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShielderFlyingState : EntityLaunchState
{
    [field: SerializeField] public LayerMask ShielderFlyingLayerMask { get; private set; }  
    private List<Entity> entitiesHitByCurrentLeap = new List<Entity>();

    [field: SerializeField] public float AttackContactDamagePercent { get; private set; } = 500f;

    private Entity rememberedTarget;
    private Vector3 attackDirection;

    private Shielder shielder;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);
        shielder = entity as Shielder;
    }

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public void SetAttackDirection(Vector3 direction)
    {
        attackDirection = direction;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        SetLaunchSettings(attackDirection, force, shielder.ShielderStunTime);

        entitiesHitByCurrentLeap.Clear();
    }

    public override void OnUpdate()
    {
        Debug.Log("Collision Detectable");
        base.OnUpdate();
        
           
        shielder.ApplyGravity();
        shielder.ApplyHorizontalVelocity();

        shielder.CheckCollisions(AttackContactDamagePercent, ref entitiesHitByCurrentLeap);
    }

    
}

