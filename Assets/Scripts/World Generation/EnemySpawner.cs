using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private WorldManager worldManager;
    [SerializeField, Self] private LandManager landManager;
    [SerializeField, Scene] private EventManager eventManager;
    [SerializeField] private List<Enemy> enemyPrefabs = new List<Enemy>();
    private ObjectPooler enemyPooler;
    private List<float> enemyNormalizedWeights = new List<float>();

    [Header("Settings")]
    [SerializeField] private float weightingSkewFactor = 2.2f;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float baseCurrency;
    [SerializeField] private float growthFactor;
    [SerializeField] private int polynomialDegree;
    private float maxShopCurrency;
    private float currentShopCurrency;

    [HideInInspector] public bool CanSpawn = false;
    private List<Enemy> enemiesSpawned = new List<Enemy>();
    private bool isSpawnDelayed = false;

    //Zone wave variables
    public bool IsInZone = false;
    //Priority wave variables
    public bool IsPriority = false;
    //Escort wave variables
    public bool NpcPresent = false; //MAKE SURE THAT THIS VALUE GETS SET BACK TO FALSE FOR ALL LANDS ONCE THE WAVE IS OVER
    private float CurrencyResetTimer;
    private float CurrencyResetTimerLength = 15f;
    public bool CurrencyTimerActive = false;

    // Ensures references are correctly assigned and validated when the script is loaded or values are changed in the Inspector.
    private void OnValidate()
    {
        this.ValidateRefs();
    }

    // Initializes key references and calculations when the object is created.
    private void Awake()
    {
        enemyPooler = transform.parent.GetComponentInChildren<ObjectPooler>();
        CalculateNormalizedWeights();
    }

    // Resets the wave settings and starts the spawn coroutine when the game starts.
    void Start()
    {
        WaveReset();
        StartCoroutine(SpawnCoroutine());
    }

    // Continuously checks for wave status and currency depletion.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) ClearWave();

        //Code to handle resetting currency during escort wave
        if(CurrencyTimerActive)
        {
            if (CurrencyResetTimer > 0)
            {
                CurrencyResetTimer -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Timer Ended");
                currentShopCurrency = maxShopCurrency;
                CurrencyTimerActive = false;
                CanSpawn = true;
            }
        }
        
    }

    // Coroutine that continuously spawns enemies at intervals, taking into account spawn settings and delays.
    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            if (!CanSpawn)
            {
                yield return null;
                continue;
            }

            if (isSpawnDelayed)
            {
                yield return new WaitForSeconds(2f);
                isSpawnDelayed = false;
            }

            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Selects an enemy to spawn based on random selection and available currency.
    private void SpawnEnemy()
    {
        float randomValue = Random.Range(0f, 1f);
        int spawnLocation = Random.Range(1, 5);
        float cumulativeWeight = 0f;

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            cumulativeWeight += enemyNormalizedWeights[i];

            if (currentShopCurrency < enemyPrefabs[i].Cost) continue;

            if (randomValue < cumulativeWeight)
            {

                if(eventManager.CurrentWaveType == WorldEvent.START)
                {
                    CreateEnemy(i);
                    currentShopCurrency -= enemyPrefabs[i].Cost;
                }

                if (eventManager.CurrentWaveType == WorldEvent.SURVIVAL)
                {
                    CreateEnemy(i);
                    currentShopCurrency -= enemyPrefabs[i].Cost;
                }

                if (eventManager.CurrentWaveType == WorldEvent.PRIORITIES)
                {
                    CreateEnemy(i);
                    currentShopCurrency -= enemyPrefabs[i].Cost;
                }

                if (eventManager.CurrentWaveType == WorldEvent.ESCORT)
                {
                    if(!NpcPresent) { return; }
                    else
                    {
                        CreateEnemy(i);
                        currentShopCurrency -= enemyPrefabs[i].Cost;
                    }
                }

                if (eventManager.CurrentWaveType == WorldEvent.DEFEND)
                {
                    currentShopCurrency -= enemyPrefabs[i].Cost;
                }

                if (eventManager.CurrentWaveType == WorldEvent.ZONES)
                {
                    CreateEnemy(i);
                    currentShopCurrency -= enemyPrefabs[i].Cost;
                }

                if (eventManager.CurrentWaveType == WorldEvent.VISIT_ALL)
                {
                    currentShopCurrency -= enemyPrefabs[i].Cost;
                }

                if (currentShopCurrency <= 0)
                {
                    CanSpawn = false;
                    if(eventManager.CurrentWaveType == WorldEvent.ESCORT)
                    {
                        CurrencyResetTimer = CurrencyResetTimerLength;
                        CurrencyTimerActive = true;
                    }
                }
                    
                




                break;
            }
        }
    }

    private void CreateEnemy(int enemy_index)
    {
        enemyPooler.ChangePrefab(enemyPrefabs[enemy_index].gameObject);

        Enemy e = enemyPooler.SpawnObject<Enemy>(landManager.GetRandomEnemySpawn().position);
        e.Init(this);

        enemiesSpawned.Add(e);
    }

    // Clears all currently spawned enemies and resets currency.
    private void ClearWave()
    {
        currentShopCurrency = 0;

        foreach (Enemy enemy in new List<Enemy>(enemiesSpawned))
        {
            enemyPooler.ReleaseObject(enemy.gameObject);
        }
        enemiesSpawned.Clear();

        CanSpawn = false;
        if (eventManager.CurrentWaveType == WorldEvent.PRIORITIES && IsPriority)
        {
            eventManager.DecrementActivePrioritiesCount();
        }
        else if (eventManager.CurrentWaveType == WorldEvent.ESCORT)
        {
            //Do Nothing
        }
        else
        {
            eventManager.DecrementActiveLandCount();
        }
    }

    // Resets wave settings and initializes currency based on the current level of the land manager.
    public void WaveReset()
    {
        if (landManager.Level <= 0)
        {
            ClearWave();
            return;
        }

        maxShopCurrency = baseCurrency + (growthFactor * Mathf.Pow(landManager.Level, polynomialDegree));
        currentShopCurrency = maxShopCurrency;

        CanSpawn = true;
        isSpawnDelayed = true;
    }

    // Calculates normalized weights for each enemy based on their cost and a skew factor.
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

    // Removes a specific enemy from the spawned list and checks for wave completion.
    public void RemoveEnemyFromList(Enemy e)
    {
        enemiesSpawned.Remove(e);

        if (currentShopCurrency > 0) return;

        if (enemiesSpawned.Count == 0)
        {
            if (eventManager.CurrentWaveType == WorldEvent.PRIORITIES && IsPriority)
            {
                eventManager.DecrementActivePrioritiesCount();
            }
            else if (eventManager.CurrentWaveType == WorldEvent.ESCORT)
            {
                //do nothing
            }
            else if (eventManager.CurrentWaveType == WorldEvent.ZONES)
            {
                //do nothing
            }
            else
            {
                eventManager.DecrementActiveLandCount();
            }
        }
    }

    // Removes all spawned enemies and clears the list.
    public void DespawnAllEnemies()
    {
        foreach (Enemy enemy in new List<Enemy>(enemiesSpawned))
        {
            enemyPooler.ReleaseObject(enemy.gameObject);
        }
        enemiesSpawned.Clear();
    }
}

