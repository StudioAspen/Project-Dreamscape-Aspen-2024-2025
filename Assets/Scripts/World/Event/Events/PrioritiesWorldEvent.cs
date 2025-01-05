using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 3 Lands of the highest Level are selected.
// All lands will spawn enemies, once the enemies spawned from the specific lands chosen are defeated trigger EOW
public class PrioritiesWorldEvent : WorldEvent
{
    private List<LandManager> affectedLands = new List<LandManager>();
    private int activeLands;

    private List<GameObject> debugSpheres = new List<GameObject>();

    public PrioritiesWorldEvent(EventManager eventManager, WorldManager worldManager, EventsConfigSO eventsConfigSO) : base(eventManager, worldManager, eventsConfigSO){}

    public override void OnStarted()
    {
        activeLands = 0;

        // Get the top 3 lands based on their level and start the enemy spawners on them if they have positive levels
        affectedLands = GetTop3Lands();
        foreach (LandManager land in affectedLands)
        {
            if (land.Level <= 0) continue;

            StartEnemySpawnerWithCurrency(land);

            // Track when the enemy spawner is depleted to decrement the activeLands counter
            land.EnemySpawner.OnSpawnerDepleted += EnemySpawner_OnSpawnerDepleted;

            activeLands++;
        }

        if (activeLands <= 0)
        {
            eventManager.ClearEvent();
        }
    }

    public override void OnCleared()
    {
        StopEnemySpawners();

        foreach (LandManager land in affectedLands)
        {
            land.EnemySpawner.KillAll();

            // Unsubscribe from the OnSpawnerDepleted event for each of the affected lands
            land.EnemySpawner.OnSpawnerDepleted -= EnemySpawner_OnSpawnerDepleted;
        }
        affectedLands.Clear();

        foreach (GameObject sphere in debugSpheres)
        {
            GameObject.Destroy(sphere);
        }
        debugSpheres.Clear();
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
}