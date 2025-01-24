using System.Collections.Generic;
using UnityEngine;

public class WorldEventSO : ScriptableObject
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
    /// The list of enemy spawning coroutines that are currently running.
    /// Add coroutines that are started through the eventManager mono behaviour here.
    /// </summary>
    private protected List<Coroutine> enemySpawningCoroutines = new List<Coroutine>();

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
            Transform mainCanvasTransform = GameObject.FindGameObjectWithTag("Main Canvas").transform;
            if (mainCanvasTransform == null)
            {
                Debug.LogError("Main Canvas not found, cannot instantiate event UI");
                return;
            }

            eventUI = GameObject.Instantiate(EventUIPrefab, mainCanvasTransform);
        }
    }

    /// <summary>
    /// Called once when starting the event.
    /// </summary>
    private protected virtual void OnStarted()
    {

    }

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
    private protected virtual void OnCleared()
    {
        
    }

    /// <summary>
    /// Called every frame to update the event.
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// Spawns enemies with currency on the specified land.
    /// Populates the enemySpawningCoroutines list with the coroutine from the land's enemy spawner.
    /// </summary>
    /// <param name="land">The land to spawn enemies on.</param>
    /// <param name="willRefillCurrency">Whether to refill currency.</param>
    public void StartEnemySpawnerWithCurrency(LandManager land, bool willRefillCurrency = true)
    {
        EnemySpawner enemySpawner = land.EnemySpawner;
        enemySpawningCoroutines.Add(eventManager.StartCoroutine(enemySpawner.SpawnWithCurrencyCoroutine(willRefillCurrency)));
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
    /// /// <param name="willRefillCurrency">Whether to refill currency.</param>
    public void StartEnemySpawnersWithCurrency(List<LandManager> lands, bool willRefillCurrency = true)
    {
        foreach (LandManager land in lands)
        {
            StartEnemySpawnerWithCurrency(land, willRefillCurrency);
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
