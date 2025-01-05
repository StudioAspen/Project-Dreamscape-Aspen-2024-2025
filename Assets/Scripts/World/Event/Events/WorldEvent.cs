using System.Collections.Generic;
using UnityEngine;

public class WorldEvent
{
    /// <summary>
    /// The event manager that manages the event.
    /// Change and clear events here.
    /// Add coroutines through this mono behaviour.
    /// </summary>
    private protected EventManager eventManager;
    /// <summary>
    /// The world manager that manages the lands and grid. Use this to access information about lands.
    /// </summary>
    private protected WorldManager worldManager;
    /// <summary>
    /// The events config scriptable object that contains the default configs for events.
    /// </summary>
    private protected EventsConfigSO eventsConfigSO;

    /// <summary>
    /// The list of enemy spawning coroutines that are currently running.
    /// Add coroutines that are started through the eventManager mono behaviour here.
    /// </summary>
    private protected List<Coroutine> enemySpawningCoroutines = new List<Coroutine>();

    public WorldEvent(EventManager eventManager, WorldManager worldManager, EventsConfigSO eventsConfigSO)
    {
        this.eventManager = eventManager;
        this.worldManager = worldManager;
        this.eventsConfigSO = eventsConfigSO;
    }

    /// <summary>
    /// Called once when starting the event.
    /// </summary>
    public virtual void OnStarted() { }

    /// <summary>
    /// Called once when the event is cleared.
    /// </summary>
    public virtual void OnCleared() { }

    /// <summary>
    /// Called every frame to update the event.
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// Spawns enemies with currency on the specified land.
    /// Populates the enemySpawningCoroutines list with the coroutine from the land's enemy spawner.
    /// </summary>
    /// <param name="land">The land to spawn enemies on.</param>
    public void StartEnemySpawnerWithCurrency(LandManager land)
    {
        EnemySpawner enemySpawner = land.EnemySpawner;
        enemySpawningCoroutines.Add(eventManager.StartCoroutine(enemySpawner.SpawnWithCurrencyCoroutine()));
    }

    /// <summary>
    /// Spawns enemies with a specified duration on the specified land.
    /// Populates the enemySpawningCoroutines list with the coroutine from the land's enemy spawner.
    /// </summary>
    /// <param name="land">The land to spawn enemies on.</param>
    /// <param name="duration">The duration of how long the enemies will spawn for.</param>
    public void StartEnemySpawnerWithDuration(LandManager land, float duration)
    {
        EnemySpawner enemySpawner = land.EnemySpawner;
        enemySpawningCoroutines.Add(eventManager.StartCoroutine(enemySpawner.SpawnWithDurationCoroutine(duration)));
    }

    /// <summary>
    /// Spawns enemies with currency on the specified lands.
    /// Populates the enemySpawningCoroutines list with the coroutines from each land's enemy spawner.
    /// </summary>
    /// <param name="lands">The list of lands to spawn enemies on.</param>
    public void StartEnemySpawnersWithCurrency(List<LandManager> lands)
    {
        foreach (LandManager land in lands)
        {
            StartEnemySpawnerWithCurrency(land);
        }
    }

    /// <summary>
    /// Spawns enemies with a specified duration on the specified lands.
    /// Populates the enemySpawningCoroutines list with the coroutines from each land's enemy spawner.
    /// </summary>
    /// /// <param name="lands">The list of lands to spawn enemies on.</param>
    /// /// <param name="duration">The duration of how long the enemies will spawn for.</param>
    public void StartEnemySpawnersWithDuration(List<LandManager> lands, float duration)
    {
        foreach (LandManager land in lands)
        {
            StartEnemySpawnerWithDuration(land, duration);
        }
    }

    /// <summary>
    /// Stops and clears all enemy spawning coroutines, stopping all enemy spawning on all lands.
    /// </summary>
    public void StopEnemySpawners()
    {
        foreach (Coroutine coroutine in enemySpawningCoroutines)
        {
            if (coroutine != null) eventManager.StopCoroutine(coroutine);
        }
        enemySpawningCoroutines.Clear();
    }
}
