using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShielderFlyingState : EntityLaunchState
{
    private Shielder shielder;

    [field: SerializeField] public LayerMask ShielderFlyingLayerMask { get; private set; }  
    private List<Entity> entitiesHit = new List<Entity>();

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

        entitiesHit.Clear();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        CheckCollisions(ContactDamageMultiplier, ref entitiesHit);
    }

    private void CheckCollisions(float damagePercent, ref List<Entity> hitEntities)
    {
        List<Collider> hits = shielder.GetCustomCollisionHits(ShielderFlyingLayerMask);

        foreach (Collider hit in hits)
        {
            if (shielder.DidHitEnemyEntity(hit, out Entity enemyEntity))
            {
                if (hitEntities.Contains(enemyEntity)) continue;
                hitEntities.Add(enemyEntity);

                shielder.DealDamageToOtherEntity(enemyEntity, shielder.CalculateDamage(damagePercent), hit.ClosestPoint(shielder.GetColliderCenterPosition()));
                return;
            }
        }
    }
}