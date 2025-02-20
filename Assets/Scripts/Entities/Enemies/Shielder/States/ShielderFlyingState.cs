using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShielderFlyingState : EntityLaunchState
{
    private Shielder shielder;

    [field: SerializeField] public LayerMask ShielderFlyingLayerMask { get; private set; }  
    private List<Entity> entitiesHitByCurrentLeap = new List<Entity>();

    [field: SerializeField] public float ContactDamageMultiplier { get; private set; } = 1f;
    [field: SerializeField] public float ContactStunDuration { get; private set; } = 1f;

    public override void Init(Entity entity)
    {
        base.Init(entity);
        shielder = entity as Shielder;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        stunDuration = ContactStunDuration;

        entitiesHitByCurrentLeap.Clear();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        shielder.CheckCollisions(ContactDamageMultiplier, ref entitiesHitByCurrentLeap);
    }
}