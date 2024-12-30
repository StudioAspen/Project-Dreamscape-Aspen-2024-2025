using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 3 Lands of the highest Level are selected.
// All lands will spawn enemies, once the enemies spawned from the specific lands chosen are defeated trigger EOW
public class PrioritiesWorldEvent : BaseState
{
    private EventManager eventManager;
    private WorldManager worldManager;
    private EventsConfigSO eventsConfigSO;

    private List<LandManager> affectedLands = new List<LandManager>();
    private List<Coroutine> enemySpawningCoroutines = new List<Coroutine>();
    private int activeLands;

    private List<GameObject> debugSpheres = new List<GameObject>();

    public PrioritiesWorldEvent(EventManager eventManager, WorldManager worldManager, EventsConfigSO eventsConfigSO)
    {
        this.eventManager = eventManager;
        this.worldManager = worldManager;
        this.eventsConfigSO = eventsConfigSO;
    }

    public override void OnEnter()
    {
        activeLands = 0;

        affectedLands = GetTop3Lands();
        foreach (LandManager land in affectedLands)
        {
            if (land.Level <= 0) continue;

            EnemySpawner enemySpawner = land.EnemySpawner;
            enemySpawningCoroutines.Add(eventManager.StartCoroutine(enemySpawner.SpawnWithCurrencyCoroutine()));

            enemySpawner.OnSpawnerDepleted += EnemySpawner_OnSpawnerDepleted;

            activeLands++;
        }

        if (activeLands <= 0)
        {
            eventManager.ClearEvent();
        }
    }

    public override void OnExit()
    {
        foreach (Coroutine coroutine in enemySpawningCoroutines)
        {
            if (coroutine != null) eventManager.StopCoroutine(coroutine);
        }
        enemySpawningCoroutines.Clear();

        foreach (LandManager land in affectedLands)
        {
            land.EnemySpawner.KillAll();

            land.EnemySpawner.OnSpawnerDepleted -= EnemySpawner_OnSpawnerDepleted;
        }
        affectedLands.Clear();

        foreach (GameObject sphere in debugSpheres)
        {
            GameObject.Destroy(sphere);
        }
        debugSpheres.Clear();
    }

    public override void Update()
    {

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

            debugSpheres.Add(CustomDebug.InstantiateTemporarySphere(sortedLands[i].transform.position + 15f * Vector3.up, 3f, Mathf.Infinity, new Color(1, 0, 0, 0.25f)));
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