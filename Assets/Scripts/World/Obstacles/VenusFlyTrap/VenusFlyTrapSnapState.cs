using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VenusFlyTrapSnapState : VenusFlyTrapBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public Vector2Int DamageRange { get; private set; } = new Vector2Int(15, 25);
    [field: SerializeField] public float DamageTickDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float ReleaseDuration { get; private set; } = 1.5f;

    private Dictionary<Entity, float> trappedEntities = new();
    private float timer;
    private float AttackDuration => 1.5f;

    public override void OnEnter()
    {
        timer = 0f;
        trappedEntities.Clear();
        // Play bite animation here if needed
    }

    public override void OnExit()
    {
        trappedEntities.Clear();
    }

    public override void OnUpdate()
{
    timer += Time.deltaTime;
    
    foreach (Entity entity in trappedEntities.Keys.ToList())
    {
        if (trappedEntities.TryGetValue(entity, out float remainingTime))
        {
            if (remainingTime <= 0)
            {
                int damage = Random.Range(DamageRange.x, DamageRange.y);
                entity.TakeDamage(damage, entity.GetColliderCenterPosition(), gameObject);
                trappedEntities[entity] = DamageTickDuration;
            }
            else
            {
                trappedEntities[entity] -= Time.deltaTime;
            }
        }
    }

    // Transition to ReleaseState after the attack duration is over
    if (timer >= AttackDuration) 
    {
        timer += Time.deltaTime;
        if (timer >= ReleaseDuration)
        {
            Debug.Log("Venus Fly Trap is releasing the target!");
                venusFlyTrap.ChangeState(venusFlyTrap.VenusFlyTrapIdleState);
        }
    }
}

    public void OnTriggerStay(Collider other)
    {
        Entity hitEntity = other.gameObject.GetComponent<Player>();
        if (hitEntity == null) return;

        if (!trappedEntities.ContainsKey(hitEntity))
        {
            trappedEntities.Add(hitEntity, 0);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Entity hitEntity = other.gameObject.GetComponent<Player>();
        if (hitEntity == null) return;

        if (trappedEntities.ContainsKey(hitEntity))
        {
            trappedEntities.Remove(hitEntity);
        }
    }
}
