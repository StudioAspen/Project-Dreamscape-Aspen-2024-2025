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

        entity.OnEntityDeath += Entity_OnEntityDeath;
    }

    private void OnDestroy()
    {
        entity.OnEntityDeath -= Entity_OnEntityDeath;
    }

    private void Entity_OnEntityDeath(GameObject killer)
    {
        ShardCollectible spawnedShard = Instantiate(ShardPrefab, entity.GetColliderCenterPosition(), Quaternion.identity);
        spawnedShard.Init(entity.GetType(), color, memoryAbility, GetShardDropCount());
    }

    private int GetShardDropCount()
    {
        int finalShardDropCount = shardDropCount;
        if (gameObject.TryGetComponent(out EntityStatusEffector entityStatusEffector))
        {
            if (entityStatusEffector.CurrentStatusEffects.Values.OfType<EliteVariantStatusEffectSO>().FirstOrDefault() == null) return finalShardDropCount;
            finalShardDropCount = Mathf.RoundToInt(shardDropCount * eliteShardDropCountMultiplier);
            Debug.Log($"Elite holder, multiplying shard drop count by 1.5x: {shardDropCount} -> {finalShardDropCount}");
        }

        return finalShardDropCount;
    }
}
