using System.Linq;
using UnityEngine;

public class ShardHolder : MonoBehaviour
{
    private Entity entity;

    [field: SerializeField] public ShardCollectible ShardPrefab { get; private set; }

    [Header("Config")]
    [SerializeField] private Color color = Color.white;
    [SerializeField] private PlayerAbilityStateSO memoryAbility;
    [SerializeField] private int shardDropCount = 1;
    [SerializeField] private float eliteShardDropCountMultiplier = 1.5f;

    private void Awake()
    {
        entity = GetComponent<Entity>(); 
    }

    private void OnEnable()
    {
        entity.OnEntityDeath += Entity_OnEntityDeath;
    }

    private void OnDisable()
    {
        entity.OnEntityDeath -= Entity_OnEntityDeath;
    }

    private void Entity_OnEntityDeath(GameObject killer)
    {
        if (killer == null) return;
        if(!killer.TryGetComponent(out MemorySystem memorySystem)) return; // Player must last hit the enemy to get shard drop
        if (Slime.IsEntityACloneSlime(entity)) return; // Cloned slimes won't drop

        ShardCollectible spawnedShard = Instantiate(ShardPrefab, entity.GetColliderCenterPosition(), Quaternion.identity);
        spawnedShard.Init(entity.GetType(), color, memoryAbility, GetShardDropCount());
    }

    /// <summary>
    /// Calculates how many shards the shard holder will drop taking into consideration the elite multiplier.
    /// </summary>
    private int GetShardDropCount()
    {
        int finalShardDropCount = shardDropCount;
        if (gameObject.TryGetComponent(out EntityStatusEffector entityStatusEffector))
        {
            if (entityStatusEffector.CurrentStatusEffects.Values.OfType<EliteVariantStatusEffectSO>().FirstOrDefault() == null) return finalShardDropCount;
            finalShardDropCount = Mathf.RoundToInt(shardDropCount * eliteShardDropCountMultiplier);
            //Debug.Log($"Elite holder, multiplying shard drop count by 1.5x: {shardDropCount} -> {finalShardDropCount}");
        }

        return finalShardDropCount;
    }
}
