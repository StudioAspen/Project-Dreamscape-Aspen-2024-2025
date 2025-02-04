using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 3 Lands of the highest Level are selected.
// All lands will spawn enemies, once the enemies spawned from the specific lands chosen are defeated trigger EOW
[CreateAssetMenu(fileName = "Priorities World Event", menuName = "World Event/Priorities")]
public class PrioritiesWorldEventSO : WorldEventSO
{
    [field: Header("Config")]
    [field: SerializeField] public int PrioritiesEventDummyVariable { get; private set; }

    private List<LandManager> affectedLands = new List<LandManager>();
    private int activeLands;

    private List<GameObject> debugSpheres = new List<GameObject>();
    private List<GameObject> enemyDebugSpheres = new List<GameObject>();

    private protected override void OnStarted()
    {
        activeLands = 0;

        // Spawn enemies on all lands
        StartEnemySpawnersWithCurrency(worldManager.SpawnedLands.Values.ToList());

        // Get the top 3 lands based on their level and track them
        affectedLands = GetTop3Lands();
        foreach (LandManager land in affectedLands)
        {
            if (land.Level <= 0) continue;

            // Track when the enemy spawner is depleted to decrement the activeLands counter
            land.EnemySpawner.OnSpawnerDepleted += EnemySpawner_OnSpawnerDepleted;

            // Track when the enemy spawner spawns an enemy to add special indicators to them
            land.EnemySpawner.OnEnemySpawned += EnemySpawner_OnEnemySpawned;

            activeLands++;
        }

        if (activeLands <= 0)
        {
            eventManager.ClearEvent();
        }
    }

    private protected override void OnCleared()
    {
        StopEnemySpawners();

        foreach(LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.DeactivateAllEnemies();
        }

        foreach (LandManager land in affectedLands)
        {
            // Unsubscribe from the OnSpawnerDepleted event for each of the affected lands
            land.EnemySpawner.OnSpawnerDepleted -= EnemySpawner_OnSpawnerDepleted;

            // Unsubscribe from the OnEnemySpawned event for each of the affected lands
            land.EnemySpawner.OnEnemySpawned -= EnemySpawner_OnEnemySpawned;
        }
        affectedLands.Clear();

        foreach (GameObject sphere in debugSpheres)
        {
            GameObject.Destroy(sphere);
        }
        debugSpheres.Clear();

        foreach (GameObject sphere in enemyDebugSpheres)
        {
            GameObject.Destroy(sphere);
        }
        enemyDebugSpheres.Clear();
    }

    /// <summary>
    /// Gets the top 3 lands based on their level.
    /// </summary>
    /// <returns>A list of the top 3 lands.</returns>
    private List<LandManager> GetTop3Lands()
    {
        List<LandManager> topLands = new List<LandManager>();

        // Sort the lands by level in descending order
        List<LandManager> sortedLands = worldManager.SpawnedLands.Values.OrderByDescending(land => land.Level).ToList();

        // Add the top 3 lands to the result list
        for (int i = 0; i < 3 && i < sortedLands.Count; i++)
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
        GameObject enemyDebugSphere = CustomDebug.InstantiateTemporarySphere(enemySpawned.transform.position + 3f * Vector3.up, 0.5f, Mathf.Infinity, new Color(1, 0, 0, 0.25f));
        enemyDebugSphere.transform.SetParent(enemySpawned.transform);

        enemyDebugSpheres.Add(enemyDebugSphere);
    }
}