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
    private ObjectPooler enemyPooler;
    private ObjectPooler materializeVFXPooler;

    [field: Header("References")]
    [field: SerializeField] public List<Enemy> NeutralEnemyPrefabs { get; private set; } = new List<Enemy>();
    [SerializeField] private List<Transform> enemySpawnPoints;
    private List<float> enemyNormalizedWeights = new List<float>();

    [Header("Currency Settings")]
    [SerializeField] private float weightingSkewFactor = 2.2f;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float baseCurrency;
    [SerializeField] private float growthFactor;
    [SerializeField] private int polynomialDegree;
    [SerializeField] private float eliteChance = 0.5f;
    private float maxShopCurrency;
    private float currentShopCurrency;
    private bool isUsingCurrency;
    /// <summary>
    /// Triggers when the spawner has no more currency and all enemies are defeated.
    /// </summary>
    public Action OnSpawnerDepleted = delegate { };

    private List<Enemy> enemiesSpawned = new List<Enemy>();
    /// <summary>
    /// Triggers when the spawner spawns an enemy.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Enemy spawnedEnemy</c>: The enemySpawned.</description></item>
    /// </list>
    /// </remarks>
    public Action<Enemy> OnEnemySpawned = delegate { };

    private void Awake()
    {
        landManager = GetComponent<LandManager>();
        worldManager = FindObjectOfType<WorldManager>();

        enemyPooler = worldManager.transform.Find("EnemyPooler").GetComponent<ObjectPooler>();
        materializeVFXPooler = worldManager.transform.Find("MaterializeVFXPooler").GetComponent<ObjectPooler>();

        CalculateNormalizedWeights();
    }

    /// <summary>
    /// Spawns enemies with currency coroutine.
    /// </summary>
    /// <returns>An IEnumerator for the coroutine.</returns>
    /// <param name="willRefillCurrency">Whether to refill currency.</param>
    public IEnumerator SpawnWithCurrencyCoroutine(bool willRefillCurrency = true)
    {
        isUsingCurrency = true;
        if(willRefillCurrency) RefillCurrency();

        while (currentShopCurrency > 0)
        {
            yield return new WaitForSeconds(spawnInterval);

            SpawnRandomEnemy(true);
        }
    }

    /// <summary>
    /// Spawns enemies for a specified duration.
    /// This does not use currency.
    /// </summary>
    /// <param name="duration">The duration of the spawning process.</param>
    public IEnumerator SpawnWithDurationCoroutine(float duration)
    {
        float elapsedTime = 0f;
        float spawnTimer = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                SpawnRandomEnemy(false);
                spawnTimer = 0f;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Spawns a random enemy based on weighted probabilities.
    /// </summary>
    /// <param name="willUseCurrency">Flag indicating whether currency should be used for spawning.</param>
    private void SpawnRandomEnemy(bool willUseCurrency)
    {
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        int spawnLocation = UnityEngine.Random.Range(1, 5);
        float cumalativeWeight = 0f;

        for (int i = 0; i < NeutralEnemyPrefabs.Count; i++)
        {
            cumalativeWeight += enemyNormalizedWeights[i];

            if (willUseCurrency && currentShopCurrency < NeutralEnemyPrefabs[i].Cost) continue;

            if (randomValue < cumalativeWeight)
            {
                Enemy spawnedEnemy = SpawnEnemy(NeutralEnemyPrefabs[i], GetRandomEnemySpawnPointTransform().position);

                // If the spawned enemy is an elite, apply a random elite status effect
                if (UnityEngine.Random.value < eliteChance)
                {
                    int randomIndex = UnityEngine.Random.Range(0, worldManager.EliteVariantStatusEffects.Count);
                    EliteVariantStatusEffectSO eliteStatusEffectToApply = worldManager.EliteVariantStatusEffects[randomIndex];

                    EntityStatusEffector.TryApplyStatusEffect(spawnedEnemy.gameObject, eliteStatusEffectToApply, spawnedEnemy.gameObject);
                }

                MaterializeEntity(spawnedEnemy);

                if (willUseCurrency)
                {
                    currentShopCurrency -= NeutralEnemyPrefabs[i].Cost;
                }

                break;
            }
        }
    }

    /// <summary>
    /// Spawns an enemy based on the provided enemy prefab.
    /// </summary>
    /// <param name="enemyPrefab">The enemy prefab to spawn.</param>
    public Enemy SpawnEnemy(Enemy enemyPrefab, Vector3 spawnPosition)
    {
        enemyPooler.ChangePrefab(enemyPrefab.gameObject);

        Enemy spawnedEnemy = enemyPooler.SpawnObject<Enemy>(spawnPosition);
        spawnedEnemy.Init(this);

        OnEnemySpawned?.Invoke(spawnedEnemy);

        enemiesSpawned.Add(spawnedEnemy);

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

        MaterializeVFX materializeVFX = materializeVFXPooler.SpawnObject<MaterializeVFX>(entity.transform.position);

        for(int i = 0; i < entityRendererManager.Renderers.Count; i++)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = entityRendererManager.Renderers[i] as SkinnedMeshRenderer;
            if (skinnedMeshRenderer == null) continue;

            // If the current renderer is the last one, invoke the callback
            materializeVFX.TriggerMaterializeVFX(skinnedMeshRenderer, i == entityRendererManager.Renderers.Count - 1);
        }
    }

    /// <summary>
    /// Refills the current shop currency by calculating the shop currency based on the base currency, growth factor, and polynomial degree.
    /// </summary>
    private void RefillCurrency()
    {
        currentShopCurrency = CalculateShopCurrency();
    }

    /// <summary>
    /// Generates a random spawn point for an enemy.
    /// </summary>
    /// <returns>The position of the random spawn point.</returns>
    private Transform GetRandomEnemySpawnPointTransform()
    {
        int randomIndex = UnityEngine.Random.Range(0, enemySpawnPoints.Count);
        return enemySpawnPoints[randomIndex];
    }

    /// <summary>
    /// Calculates the shop currency based on the base currency, growth factor, and polynomial degree.
    /// </summary>
    /// <returns>The calculated shop currency.</returns>
    private float CalculateShopCurrency() => baseCurrency + (growthFactor * Mathf.Pow(landManager.Level, polynomialDegree));

    /// <summary>
    /// Calculates the normalized weights for each enemy prefab based on their cost and the weighting skew factor.
    /// Used for weighted random selection of enemy prefabs.
    /// Higher cost enemies are less likely to be selected.
    /// </summary>
    private void CalculateNormalizedWeights()
    {
        enemyNormalizedWeights = new List<float>();

        float totalWeight = 0f;

        foreach (Enemy enemy in NeutralEnemyPrefabs)
        {
            totalWeight += 1f / Mathf.Pow(enemy.Cost, weightingSkewFactor);
        }

        for (int i = 0; i < NeutralEnemyPrefabs.Count; i++)
        {
            float weight = 1f / Mathf.Pow(NeutralEnemyPrefabs[i].Cost, weightingSkewFactor);

            enemyNormalizedWeights.Add(weight / totalWeight);
        }
    }

    /// <summary>
    /// Removes the specified enemy from the list of spawned enemies.
    /// </summary>
    /// <param name="enemy">The enemy to remove.</param>
    public void RemoveEnemy(Enemy enemy)
    {
        enemiesSpawned.Remove(enemy);

        if (isUsingCurrency)
        {
            if (IsFullyCleared())
            {
                isUsingCurrency = false;
                OnSpawnerDepleted?.Invoke();
            }
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

        isUsingCurrency = false;
    }

    /// <summary>
    /// Checks if the enemy spawner has used up all of its currency and has all spawned enemies defeated.
    /// Use this method only when the enemy spawner is using currency and has a finite number of enemies to spawn.
    /// </summary>
    /// <returns>True if the enemy spawner is fully cleared, false otherwise.</returns>
    private bool IsFullyCleared() => currentShopCurrency <= 0 && enemiesSpawned.Count == 0;
}
