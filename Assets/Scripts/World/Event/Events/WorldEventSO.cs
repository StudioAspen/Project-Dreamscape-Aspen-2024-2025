using System.Collections.Generic;
using UnityEngine;

public abstract class WorldEventSO : ScriptableObject
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
    /// A list of lands that have active spawners.
    /// </summary>
    private protected List<LandManager> activeSpawnerLands = new List<LandManager>();

    [field: Header("Display")]
    [field: SerializeField] public string EventName { get; private set; } = "Event";
    [field: SerializeField, TextArea(3, 20)] public string Description { get; private set; } = "Description of the event.";
    [field: SerializeField] public WorldEventUI EventUIPrefab { get; private set; }
    private WorldEventUI eventUI;

    /// <summary>
    /// Initializes the WorldEventSO with the specified event manager, world manager, and events config scriptable object.
    /// </summary>
    /// <param name="eventManager">The event manager that manages the event.</param>
    /// <param name="worldManager">The world manager that manages the lands and grid.</param>
    /// <param name="eventsConfigSO">The events config scriptable object that contains the default configs for events.</param>
    public void Init(EventManager eventManager, WorldManager worldManager)
    {
        this.eventManager = eventManager;
        this.worldManager = worldManager;
    }

    /// <summary>
    /// Starts the event.
    /// Spawns the event UI prefab on the main canvas if a prefab is provided.
    /// </summary>
    public void Start()
    {
        OnStarted();

        if (EventUIPrefab != null)
        {
            Transform eventDisplayUITransform = GameObject.FindObjectOfType<EventDisplayUI>(true).transform;
            if (eventDisplayUITransform == null)
            {
                Debug.LogError("Main Canvas not found, cannot instantiate event UI");
                return;
            }

            eventUI = GameObject.Instantiate(EventUIPrefab, eventDisplayUITransform);
        }
    }

    /// <summary>
    /// Called once when starting the event.
    /// </summary>
    private protected abstract void OnStarted();

    /// <summary>
    /// Clears the event.
    /// Deletes the event UI prefab if it exists.
    /// </summary>
    public void Clear()
    {
        OnCleared();

        if (eventUI != null) GameObject.Destroy(eventUI.gameObject);
    }

    /// <summary>
    /// Called once when the event is cleared.
    /// </summary>
    private protected abstract void OnCleared();

    /// <summary>
    /// Upates the event.
    /// Called by the event manager.
    /// </summary>
    public void Update()
    {
        OnUpdate();
    }

    /// <summary>
    /// Called every frame to update the event.
    /// </summary>
    private protected abstract void OnUpdate();

    /// <summary>
    /// Spawns enemies with currency on the specified land.
    /// Adds the land to the activeSpawnerLands list for tracking.
    /// </summary>
    /// <param name="land">The land to spawn enemies on.</param>
    /// <param name="willRestockCurrency">Whether to restock currency.</param>
    public void StartEnemySpawnerWithCurrency(LandManager land, bool willRestockCurrency = true)
    {
        EnemySpawner enemySpawner = land.EnemySpawner;
        enemySpawner.StartSpawnerWithCurrency(willRestockCurrency);
        activeSpawnerLands.Add(land);
    }

    /// <summary>
    /// Spawns enemies with a specified duration on the specified land.
    /// Adds the land to the activeSpawnerLands list for tracking.
    /// </summary>
    /// <param name="land">The land to spawn enemies on.</param>
    /// <param name="duration">The duration of how long the enemies will spawn for.</param>
    public void StartEnemySpawnerWithDuration(LandManager land, float duration)
    {
        EnemySpawner enemySpawner = land.EnemySpawner;
        enemySpawner.StartSpawnerWithDuration(duration);
        activeSpawnerLands.Add(land);
    }

    /// <summary>
    /// Spawns enemies with currency on the specified lands.
    /// Adds the lands to the activeSpawnerLands list for tracking.
    /// </summary>
    /// <param name="lands">The list of lands to spawn enemies on.</param>
    /// <param name="willRestockCurrency">Whether to restock currency.</param>
    public void StartEnemySpawnersWithCurrency(List<LandManager> lands, bool willRestockCurrency = true)
    {
        foreach (LandManager land in lands)
        {
            StartEnemySpawnerWithCurrency(land, willRestockCurrency);
        }
    }

    /// <summary>
    /// Spawns enemies with a specified duration on the specified lands.
    /// Populates the enemySpawningCoroutines list with the coroutines from each land's enemy spawner.
    /// Adds the lands to the activeSpawnerLands list for tracking.
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
    /// Stops land from spawning enemies and removes it from the active lands list.
    /// </summary>
    /// <param name="land">The land to stop spawning.</param>
    public void StopEnemySpawner(LandManager land)
    {
        land.EnemySpawner.StopSpawner();
        activeSpawnerLands.Remove(land);
    }

    /// <summary>
    /// Stops and clears all active spawner lands.
    /// </summary>
    public void StopActiveEnemySpawners()
    {
        foreach (LandManager land in activeSpawnerLands)
        {
            land.EnemySpawner.StopSpawner();
        }
        activeSpawnerLands.Clear();
    }
}
