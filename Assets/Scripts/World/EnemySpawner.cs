using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private WorldManager worldManager;
    [SerializeField, Self] private LandManager landManager;
    [SerializeField] private List<Enemy> enemyPrefabs = new List<Enemy>();
    [SerializeField] private List<Transform> enemySpawnPoints;
    private List<float> enemyNormalizedWeights = new List<float>();
    private ObjectPooler enemyPooler;

    [Header("Currency Settings")]
    [SerializeField] private float weightingSkewFactor = 2.2f;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float baseCurrency;
    [SerializeField] private float growthFactor;
    [SerializeField] private int polynomialDegree;
    private float maxShopCurrency;
    private float currentShopCurrency;
    private bool isUsingCurrency;
    public Action OnSpawnerDepleted = delegate { }; // Triggers when the spawner has no more currency and all enemies are defeated

    private List<Enemy> enemiesSpawned = new List<Enemy>();

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        enemyPooler = worldManager.GetComponentInChildren<ObjectPooler>();

        CalculateNormalizedWeights();
    }

    /// <summary>
    /// Spawns enemies with currency coroutine.
    /// </summary>
    /// <returns>An IEnumerator for the coroutine.</returns>
    public IEnumerator SpawnWithCurrencyCoroutine()
    {
        isUsingCurrency = true;
        RefillCurrency();

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

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            cumalativeWeight += enemyNormalizedWeights[i];

            if (willUseCurrency && currentShopCurrency < enemyPrefabs[i].Cost) continue;

            if (randomValue < cumalativeWeight)
            {
                enemyPooler.ChangePrefab(enemyPrefabs[i].gameObject);

                Enemy spawnedEnemy = enemyPooler.SpawnObject<Enemy>(GetRandomEnemySpawnPointTransform().position);
                spawnedEnemy.Init(this);

                enemiesSpawned.Add(spawnedEnemy);

                if (willUseCurrency)
                {
                    currentShopCurrency -= enemyPrefabs[i].Cost;
                }

                break;
            }
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

        foreach (Enemy enemy in enemyPrefabs)
        {
            totalWeight += 1f / Mathf.Pow(enemy.Cost, weightingSkewFactor);
        }

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            float weight = 1f / Mathf.Pow(enemyPrefabs[i].Cost, weightingSkewFactor);

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
    /// Kills all spawned enemies.
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
