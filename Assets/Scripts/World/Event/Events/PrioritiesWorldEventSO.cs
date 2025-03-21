using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// 3 Lands of the highest Level are selected.
// All lands will spawn enemies, once the enemies spawned from the specific lands chosen are defeated trigger EOW
[CreateAssetMenu(fileName = "Priorities World Event", menuName = "World/World Event/Priorities")]
public class PrioritiesWorldEventSO : WorldEventSO
{
    [field: Header("Config")]
    [field: SerializeField] public Marker MarkerPrefab { get; private set; }

    [field: Space(5)]

    /// <summary>
    /// The number of lands needed on the map for the event to select an additional top land. Default value: 4
    /// </summary>
    [field: Tooltip("The number of lands needed on the map for the event to select an additional top land. Default value: 4")]
    [field: Range(2, 6)]
    [field: SerializeField] public float LandsPerTopLand { get; private set; } = 4;

    /// <summary>
    /// The number of times each top land's Enemy Spawner will refill its currency and spawn as many enemies as possible. Default value: 1
    /// </summary>
    [field: Range(1, 3)]
    [field: Tooltip("The number of times each top land's Enemy Spawner will refill its currency and spawn as many enemies as possible. Default value: 1")]
    [field: SerializeField] public int SpawnBursts { get; private set; } = 1;

    /// <summary>
    /// The time that must elapse between spawning enemies during non-timed events. Default value: 3.0 seconds.
    /// </summary>
    [field: Tooltip("The time that must elapse between spawning enemies during non-time events. Default value: 3.0 seconds")]
    [field: Range(3f, 30f)]
    [field: SerializeField] public float BaseSpawnInterval { get; private set; } = 3f;

    private List<LandManager> topLands = new List<LandManager>();
    private int activeLands;

    private List<GameObject> debugSpheres = new List<GameObject>();
    private Dictionary<Enemy, Marker> enemyMarkers = new();

    private int enemiesRemaining;
    private int totalEnemiesToKill;

    private protected override void OnStarted()
    {
        activeLands = 0;
        topLands = new();
        debugSpheres = new();
        enemyMarkers = new();

        // Get all spawned lands on the map
        List<LandManager> spawnedLands = worldManager.SpawnedLands.Values.ToList();
        enemiesRemaining = 0;
        totalEnemiesToKill = 0;

        int topLandsAmount = 1 + Mathf.FloorToInt((spawnedLands.Count - 1) / LandsPerTopLand);

        // Get the top lands based on their level and track them
        topLands = GetTopLands(topLandsAmount);

        foreach (LandManager land in topLands)
        {
            if (land.Level <= 0) continue;

            // Track when the enemy spawner is depleted to decrement the activeLands counter
            land.EnemySpawner.OnSpawnerDepleted += EnemySpawner_OnSpawnerDepleted;

            // Track when the enemy spawner spawns an enemy to add special indicators to them
            land.EnemySpawner.OnEnemySpawned += EnemySpawner_OnEnemySpawned;

            land.EnemySpawner.OnEnemyDeath += EnemySpawner_OnEnemyDeath;

            // Each top land will use ALL of its currency to spawn many enemies as possible all at once (with 0.1s delay).
            StartEnemySpawnerWithCurrency(land, 0.1f * Vector2.one, BaseSpawnAmount);
              
            activeLands++;
        }

        // Spawn on all the other lands
        foreach(LandManager land in spawnedLands)
        {
            if (topLands.Contains(land)) return; // Skip the priority land
            StartEnemySpawnerWithCurrency(land, new Vector2(BaseSpawnInterval, BaseSpawnInterval), BaseSpawnAmount);
        }

        if (activeLands <= 0)
        {
            eventManager.ClearEvent();
        }
    }

    private protected override void OnCleared()
    {
        StopActiveEnemySpawners();

        foreach(LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.DeactivateAllEnemies();
        }

        foreach (LandManager land in topLands)
        {
            // Unsubscribe from the OnSpawnerDepleted event for each of the affected lands
            land.EnemySpawner.OnSpawnerDepleted -= EnemySpawner_OnSpawnerDepleted;

            // Unsubscribe from the OnEnemySpawned event for each of the affected lands
            land.EnemySpawner.OnEnemySpawned -= EnemySpawner_OnEnemySpawned;

            land.EnemySpawner.OnEnemyDeath -= EnemySpawner_OnEnemyDeath;
        }
        topLands.Clear();

        foreach (GameObject sphere in debugSpheres)
        {
            GameObject.Destroy(sphere);
        }
        debugSpheres.Clear();

        foreach (Marker marker in enemyMarkers.Values)
        {
            GameObject.Destroy(marker.gameObject);
        }
        enemyMarkers.Clear();
    }

    private protected override void OnUpdate()
    {
        
    }

    /// <summary>
    /// Gets the top lands based on their level.
    /// </summary>
    /// <returns>A list of the top lands.</returns>
    private List<LandManager> GetTopLands(int topLandsAmount)
    {
        List<LandManager> topLands = new List<LandManager>();

        // Sort the lands by level in descending order
        List<LandManager> sortedLands = worldManager.SpawnedLands.Values.OrderByDescending(land => land.Level).ToList();

        // Add the top 3 lands to the result list
        for (int i = 0; i < topLandsAmount && i < sortedLands.Count; i++)
        {
            topLands.Add(sortedLands[i]);

            debugSpheres.Add(CustomDebug.InstantiateTemporarySphere(sortedLands[i].transform.position + 10f * Vector3.up, 3f, Mathf.Infinity, new Color(1, 0, 0, 0.25f)));
        }

        return topLands;
    }

    private void EnemySpawner_OnSpawnerDepleted()
    {
        activeLands--;

        if (activeLands <= 0)
        {
            eventManager.ClearEvent();
        }
    }

    private void EnemySpawner_OnEnemySpawned(Enemy enemySpawned)
    {
        enemiesRemaining++;
        totalEnemiesToKill++;

        Marker enemyMarker = Instantiate(MarkerPrefab, enemySpawned.GetEntityTopPosition() + 2f * Vector3.up, Quaternion.identity, enemySpawned.transform);
        enemyMarkers.Add(enemySpawned, enemyMarker);
    }

    private void EnemySpawner_OnEnemyDeath(Enemy enemy)
    {
        enemiesRemaining--;

        if (enemyMarkers.ContainsKey(enemy))
        {
            Destroy(enemyMarkers[enemy].gameObject);
            enemyMarkers.Remove(enemy);
        }
    }

    public override void UpdateEventUIElements(TMP_Text feedbackText, TMP_Text nameText, TMP_Text optionalDescriptionText)
    {
        feedbackText.text = $"{totalEnemiesToKill - enemiesRemaining}/{totalEnemiesToKill}";
        nameText.text = $"{EventProgressionUIName.ToUpper()}";
    }
}