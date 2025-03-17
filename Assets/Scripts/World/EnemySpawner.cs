using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    private LandManager landManager;
    private WorldManager worldManager;

    [field: Header("References")]
    [field: SerializeField] public BiomeVariantStatusEffectSO BiomeVariantStatusEffect { get; private set; }
    [field: SerializeField] public List<Enemy> EnemyPrefabs { get; private set; } = new();
    private Dictionary<Enemy, float> enemySpawnChanceDictionary = new();
    [SerializeField] private List<Transform> enemySpawnPoints;

    [Header("Currency Settings")]
    [SerializeField] private float weightingSkewPower = 2.2f;
    [SerializeField] private Vector2 enemySpawnIntervalRange = new Vector2(3f, 6f);
    [SerializeField] private float baseCurrency;
    [SerializeField] private float growthFactor;
    [SerializeField] private int polynomialDegree;
    [SerializeField] private float eliteChance = 0.5f;
    private float currentRemainingCurrency;
    private Coroutine currentSpawnerCoroutine;
    private List<Enemy> enemiesSpawned = new List<Enemy>();

    /// <summary>
    /// Triggers when the spawner has no more currency and all enemies are defeated.
    /// </summary>
    public Action OnSpawnerDepleted = delegate { };
    /// <summary>
    /// Triggers when the spawner spawns an enemy.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Enemy spawnedEnemy</c>: The enemy spawned.</description></item>
    /// </list>
    /// </remarks>
    public Action<Enemy> OnEnemySpawned = delegate { };
    /// <summary>
    /// Triggers when a spawned enemy dies.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Enemy killedEnemy</c>: The enemy killed.</description></item>
    /// </list>
    /// </remarks>
    public Action<Enemy> OnEnemyDeath = delegate { };

    private void Awake()
    {
        landManager = GetComponent<LandManager>();
    }

    private void Start()
    {
        worldManager = FindObjectOfType<WorldManager>();

        CalculateNormalizedWeights();
    }

    /// <summary>
    /// Starts the enemy spawner coroutine with currency.
    /// Stops any existing spawning coroutine.
    /// </summary>
    /// <param name="spawnIntervalRange"></param>
    /// <param name="spawnAmount"></param>
    /// <param name="willRestockCurrency">Whether to restock currency</param>
    public void StartSpawnerWithCurrency(Vector2 spawnIntervalRange, int spawnAmount = 1, bool willRestockCurrency = true)
    {
        StopSpawner();
        if (landManager.Level <= 0) return;
        currentSpawnerCoroutine = StartCoroutine(SpawnWithCurrencyCoroutine(willRestockCurrency ? CalculateShopCurrency() : currentRemainingCurrency, spawnIntervalRange, spawnAmount));
    }

    /// <summary>
    /// Starts the enemy spawner coroutine with duration.
    /// Stops any existing spawning coroutine.
    /// </summary>
    /// <param name="duration">The duration to spawn enemies for.</param>
    public void StartSpawnerWithDuration(Vector2 spawnIntervalRange, float duration, int spawnAmount = 1)
    {
        StopSpawner();
        if (landManager.Level <= 0) return;
        currentSpawnerCoroutine = StartCoroutine(SpawnWithDurationCoroutine(spawnIntervalRange, duration, spawnAmount));
    }

    /// <summary>
    /// Stops the enemy spawner by stopping the coroutine.
    /// </summary>
    public void StopSpawner()
    {
        if (currentSpawnerCoroutine != null) StopCoroutine(currentSpawnerCoroutine);
        currentSpawnerCoroutine = null;
    }

    /// <summary>
    /// Spawns enemies with currency coroutine.
    /// Stores the remaining currency.
    /// <param name="startingCurrency">The starting currency</param>
    /// </summary>
    private IEnumerator SpawnWithCurrencyCoroutine(float startingCurrency, Vector2 spawnIntervalRange, int spawnAmount = 1)
    {
        currentRemainingCurrency = startingCurrency;
        List<(Enemy, float)> enemyRemainingCurrencyQueue = GetEnemyPrefabQueue(startingCurrency);

        float randomDelay = UnityEngine.Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);

        int spawnCount = 0;
        foreach ((Enemy enemyPrefab, float remainingCurrency) in enemyRemainingCurrencyQueue)
        {
            SpawnEnemy(enemyPrefab, GetRandomEnemySpawnPoint(1), true, true);
            currentRemainingCurrency = remainingCurrency;
            spawnCount++;
            if(spawnCount >= spawnAmount)
            {
                spawnCount = 0;
                yield return new WaitForSeconds(randomDelay);
            }
            else
            {
                yield return null;
            }
        }
        currentSpawnerCoroutine = null;
    }

    /// <summary>
    /// Spawns enemies for a specified duration.
    /// This does not use currency.
    /// </summary>
    /// <param name="spawnIntervalRange">The random range for the spawning interval.</param>
    /// <param name="duration">The duration of the spawning process.</param>
    /// <param name="spawnAmount">The number of enemies to spawn per interval.</param>
    private IEnumerator SpawnWithDurationCoroutine(Vector2 spawnIntervalRange, float duration, int spawnAmount = 1)
    {
        List<(Enemy, float)> enemySpawnDelayQueue = GetEnemyPrefabQueue(duration, spawnIntervalRange, spawnAmount);
        foreach ((Enemy enemyPrefab, float delay) in enemySpawnDelayQueue)
        {
            SpawnEnemy(enemyPrefab, GetRandomEnemySpawnPoint(1), true, true);
            yield return new WaitForSeconds(delay);
        }
        currentSpawnerCoroutine = null;
    }

    /// <summary>
    /// Gets the list of enemies to spawn based on the total currency to use and how much currency remains after spawning.
    /// Enemy spawn chance is based on their cost.
    /// </summary>
    /// <param name="startingCurrency">The starting currency.</param>
    /// <returns>The list of enemies and how remaining currency or empty list if a warning was thrown.</returns>
    private List<(Enemy, float)> GetEnemyPrefabQueue(float startingCurrency)
    {
        if(enemySpawnChanceDictionary == null || enemySpawnChanceDictionary.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name} Enemy Spawner: Failed to get enemy queue because enemy spawn chances have not been computed.");
            return new List<(Enemy, float)>();
        }

        List<(Enemy, float)> enemyCurrencyQueue = new List<(Enemy, float)>();
        float remainingCurrency = startingCurrency;

        while(remainingCurrency > 0)
        {
            Enemy enemyPrefab = GetRandomEnemyPrefab();
            if (enemyPrefab == null)
            {
                Debug.LogWarning($"{gameObject.name} Enemy Spawner: Failed to get enemy queue because it failed to get a random enemy prefab");
                return new List<(Enemy, float)>();
            }

            remainingCurrency -= enemyPrefab.Cost;
            enemyCurrencyQueue.Add((enemyPrefab, remainingCurrency));
        }

        return enemyCurrencyQueue;
    }

    /// <summary>
    /// Gets the list of enemies to spawn based on the total duration to use.
    /// </summary>
    /// <param name="totalDuration">The total duration to spawn the enemies</param>
    /// <param name="spawnIntervalRange">The random range of spawn intervals</param>
    /// <returns>The list of enemies and their spawn delay or empty list if a warning was thrown.</returns>
    private List<(Enemy, float)> GetEnemyPrefabQueue(float totalDuration, Vector2 spawnIntervalRange, int spawnAmount = 1)
    {
        if (enemySpawnChanceDictionary == null || enemySpawnChanceDictionary.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name} Enemy Spawner: Failed to get enemy queue because enemy spawn chances have not been computed.");
            return new List<(Enemy, float)>();
        }

        List<(Enemy, float)> enemySpawnDelayQueue = new List<(Enemy, float)>();
        float remainingDuration = totalDuration;
        float randomDelay = UnityEngine.Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);

        while (remainingDuration > randomDelay)
        {
            for (int i = 0; i < spawnAmount && remainingDuration > randomDelay; i++)
            {
                Enemy enemyPrefab = GetRandomEnemyPrefab();
                if (enemyPrefab == null)
                {
                    Debug.LogWarning($"{gameObject.name} Enemy Spawner: Failed to get enemy queue because it failed to get a random enemy prefab");
                    return new List<(Enemy, float)>();
                }

                // add the first enemy in the batch with the interval, but set every interval after to be 0.1f seconds
                float delay = i == 0 ? randomDelay : 0.1f;
                enemySpawnDelayQueue.Add((enemyPrefab, delay));
                remainingDuration -= delay;
            }
        }

        return enemySpawnDelayQueue;
    }

    /// <summary>
    /// Gets a random enemy prefab based on their weighted spawn chance from cost.
    /// </summary>
    /// <returns>The random enemy prefab</returns>
    private Enemy GetRandomEnemyPrefab()
    {
        if(EnemyPrefabs.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name} Enemy Spawner: Can't get random enemy prefab because enemy prefabs list is empty!");
            return null;
        }

        float randomChance = UnityEngine.Random.Range(0f, 1f);
        float cumalativeChance = 0f;
        foreach (Enemy enemyPrefab in EnemyPrefabs)
        {
            float chance = enemySpawnChanceDictionary[enemyPrefab];
            cumalativeChance += chance;
            if (cumalativeChance >= randomChance)
            {
                return enemyPrefab;
            }
        }

        Debug.LogWarning($"{gameObject.name} Enemy Spawner: Failed to get random enemy prefab");
        return null;
    }

    /// <summary>
    /// Spawns an enemy based on the provided enemy prefab.
    /// </summary>
    /// <param name="enemyPrefab">The enemy prefab to spawn.</param>
    /// <param name="spawnPosition">The position to spawn the enemy</param>
    /// <param name="willTryElite">Flag for whether to have a change of the enemy becoming elite</param>
    /// <param name="willMaterialize">Flag for whether to materialize the enemy</param>
    public Enemy SpawnEnemy(Enemy enemyPrefab, Vector3 spawnPosition, bool willTryElite = false, bool willMaterialize = false)
    {
        Enemy spawnedEnemy = ObjectPoolerManager.Instance.SpawnPooledObject<Enemy>(enemyPrefab.gameObject, spawnPosition);
        spawnedEnemy.Init(this);

        OnEnemySpawned?.Invoke(spawnedEnemy);

        enemiesSpawned.Add(spawnedEnemy);

        if(BiomeVariantStatusEffect != null) EntityStatusEffector.TryApplyStatusEffect(spawnedEnemy.gameObject, BiomeVariantStatusEffect, spawnedEnemy.gameObject);

        if (willTryElite)
        {
            // If the spawned enemy is an elite, apply a random elite status effect
            if (UnityEngine.Random.value < eliteChance)
            {
                int randomIndex = UnityEngine.Random.Range(0, worldManager.EliteVariantStatusEffects.Count);
                EliteVariantStatusEffectSO eliteStatusEffectToApply = worldManager.EliteVariantStatusEffects[randomIndex];
                EntityStatusEffector.TryApplyStatusEffect(spawnedEnemy.gameObject, eliteStatusEffectToApply, spawnedEnemy.gameObject);
            }
        }

        if(willMaterialize) MaterializeEntity(spawnedEnemy);

        return spawnedEnemy;
    }

    /// <summary>
    /// Materializes an entity by instantiating a materialization visual effect at its position.
    /// </summary>
    /// <param name="entity">The entity to materialize.</param>
    public void MaterializeEntity(Entity entity)
    {
        EntityRendererManager entityRendererManager = entity.GetComponent<EntityRendererManager>();
        if (entityRendererManager == null) return;

        MaterializeVFX materializeVFX = ObjectPoolerManager.Instance.SpawnPooledObject<MaterializeVFX>(ObjectPoolerManager.Instance.MaterializeVFXPrefab.gameObject, entity.transform.position);

        for(int i = 0; i < entityRendererManager.Renderers.Count; i++)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = entityRendererManager.Renderers[i] as SkinnedMeshRenderer;
            if (skinnedMeshRenderer == null) continue;

            // If the current renderer is the last one, invoke the callback
            materializeVFX.TriggerMaterializeVFX(skinnedMeshRenderer, i == entityRendererManager.Renderers.Count - 1);
        }
    }

    /// <summary>
    /// Generates a random spawn point for an enemy.
    /// It picks a random point within a radius of the spawn point.
    /// </summary>
    /// <param name="radius">The radius around the spawn point transform.</param>
    /// <returns>The position of the random spawn point.</returns>
    public Vector3 GetRandomEnemySpawnPoint(float radius)
    {
        int randomIndex = UnityEngine.Random.Range(0, enemySpawnPoints.Count);
        Transform baseSpawnPoint = enemySpawnPoints[randomIndex];

        // Generate a random offset within the radius, keeping y coordinate the same
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * radius;
        Vector3 randomPosition = new Vector3(
            baseSpawnPoint.position.x + randomOffset.x,
            baseSpawnPoint.position.y + 0.25f,
            baseSpawnPoint.position.z + randomOffset.y
        );

        return randomPosition;
    }

    /// <summary>
    /// Calculates the shop currency based on the base currency, growth factor, and polynomial degree.
    /// </summary>
    /// <returns>The calculated shop currency.</returns>
    private float CalculateShopCurrency()
    {
        return baseCurrency + (growthFactor * Mathf.Pow(landManager.Level, polynomialDegree));
    }
    
    /// <summary>
    /// Calculates the normalized weights for each enemy prefab based on their cost and the weighting skew factor.
    /// Used for weighted random selection of enemy prefabs.
    /// Higher cost enemies are less likely to be selected.
    /// </summary>
    private void CalculateNormalizedWeights()
    {
        // Get total weight first
        float totalWeight = 0f;
        foreach (Enemy enemy in EnemyPrefabs)
        {
            totalWeight += 1f / Mathf.Pow(enemy.Cost, weightingSkewPower);
        }

        if(totalWeight == 0f)
        {
            Debug.LogWarning($"{gameObject.name} Enemy Spawner: Can't compute spawn chances" +
                $" because it either has no enemy prefabs or the prefabs have 0 cost");
            return;
        }

        // Get normalized weights for each enemy prefab
        foreach(Enemy enemy in EnemyPrefabs)
        {
            float weight = 1f / Mathf.Pow(enemy.Cost, weightingSkewPower);
            enemySpawnChanceDictionary.Add(enemy, weight / totalWeight);
        }
    }

    /// <summary>
    /// Determines if the spawner is done spawning and has all enemies killed.
    /// </summary>
    /// <returns>Whether the spawner is fully cleared.</returns>
    private bool IsFullyCleared()
    {
        return currentSpawnerCoroutine == null && enemiesSpawned.Count == 0;
    }

    /// <summary>
    /// Removes the specified enemy from the list of spawned enemies.
    /// </summary>
    /// <param name="enemy">The enemy to remove.</param>
    public void RemoveEnemy(Enemy enemy)
    {
        OnEnemyDeath.Invoke(enemy);

        enemiesSpawned.Remove(enemy);

        if (IsFullyCleared())
        {
            OnSpawnerDepleted.Invoke();
        }
    }

    /// <summary>
    /// Kills all spawned enemies
    /// </summary>
    public void KillAll()
    {
        foreach (Enemy enemy in new List<Enemy>(enemiesSpawned))
        {
            enemy.Kill(null);
        }
    }

    /// <summary>
    /// Deactivates all spawned enemies by calling their Die() method.
    /// </summary>
    public void DeactivateAllEnemies()
    {
        foreach (Enemy enemy in new List<Enemy>(enemiesSpawned))
        {
            enemy.TakeDamage(0, enemy.transform.position, null, false);
            enemy.Die();
        }
    }

    /// <summary>
    /// Gets the original prefab from the spawner that matches the enemy instance's type.
    /// </summary>
    /// <param name="enemyInstance">The enemy instance.</param>
    /// <returns>The enemy prefab or null if not found.</returns>
    public Enemy GetPrefabFromEnemyInstance(Enemy enemyInstance)
    {
        foreach (Enemy enemyPrefab in EnemyPrefabs)
        {
            if (enemyPrefab.GetType() == enemyInstance.GetType())
            {
                return enemyPrefab;
            }
        }

        Debug.LogWarning("Could not find spawner enemy prefab from enemy instance type.");

        return null;
    }
}
