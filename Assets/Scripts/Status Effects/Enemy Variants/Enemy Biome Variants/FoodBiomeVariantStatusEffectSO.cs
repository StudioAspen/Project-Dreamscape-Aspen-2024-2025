using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Biome Enemy Status Variants/Food Biome")]
public class FoodBiomeVariantStatusEffectSO : BiomeVariantStatusEffectSO {
    [field: Header("Biome Enemy Status Effect Config")]
    [field: SerializeField] public float MovementSpeedMultiplier { get; private set; } = 1.337f;
    [field: SerializeField][field: Range(0f,1f)] public float FoodDropChance { get; private set; } = .15f;
    [field: SerializeField] private GameObject foodCollectiblePrefab;
    
    private protected override void OnApply()
    {
        base.OnApply();
        
        enemy = entity as Enemy;
        if (enemy == null)
        {
            Debug.LogError($"{GetType()} can only be applied to an Enemy entity.");
            RemoveSelf();
            return;
        }
        
        enemy.StatusSpeedModifier.AddMultiplier(MovementSpeedMultiplier, this);
        enemy.OnEntityDeath += TrySpawnFoodCollectible;
    }

    public override void Cancel()
    {
        base.Cancel();
        
        enemy.StatusSpeedModifier.ClearBuffsFromSource(this);
        enemy.OnEntityDeath -= TrySpawnFoodCollectible;
    }

    private void TrySpawnFoodCollectible(GameObject killer) {
        if (Random.Range(0f, 1f) < FoodDropChance) {
            Debug.LogWarning("Spawning food collectible");
            // Debug.LogWarning(foodCollectiblePrefab.name);
            // Debug.LogWarning(enemy.GetColliderCenterPosition());
            Debug.LogWarning(ObjectPoolerManager.Instance);
            FoodCollectible spawnedAbility = ObjectPoolerManager.Instance.SpawnPooledObject<FoodCollectible>(foodCollectiblePrefab, enemy.GetColliderCenterPosition() );
            spawnedAbility.Init(enemy);
        }
    }
    
}