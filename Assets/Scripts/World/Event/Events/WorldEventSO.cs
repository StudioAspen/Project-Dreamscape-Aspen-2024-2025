using System.Collections.Generic;
using TMPro;
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
    [field: SerializeField] public string EventProgressionUIName { get; private set; } = "Event";
    [field: SerializeField, TextArea(3, 20)] public string Description { get; private set; } = "Description of the event.";

    [field: Header("Enemy Spawners")] 

    /// <summary>
    /// The number of enemies to spawn at each Spawn Interval. Default value: 1 Enemy
    /// </summary>
    [field: Range(1, 10)]
    [field: SerializeField] public int BaseSpawnAmount { get; private set; } = 2;

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
    /// <param name="spawnIntervalRange"></param>
    /// <param name="spawnAmount"></param>
    /// <param name="willRestockCurrency">Whether to restock currency.</param>
    public void StartEnemySpawnerWithCurrency(LandManager land, Vector2 spawnIntervalRange, int spawnAmount = 1, bool willRestockCurrency = true)
    {
        if (land == null) return;
        if (land.EnemySpawner == null) return;

        EnemySpawner enemySpawner = land.EnemySpawner;
        enemySpawner.StartSpawnerWithCurrency(spawnIntervalRange, spawnAmount, willRestockCurrency);
        activeSpawnerLands.Add(land);
    }

    /// <summary>
    /// Spawns enemies with a specified duration on the specified land.
    /// Adds the land to the activeSpawnerLands list for tracking.
    /// </summary>
    /// <param name="land">The land to spawn enemies on.</param>
    /// <param name="duration">The duration of how long the enemies will spawn for.</param>
    public void StartEnemySpawnerWithDuration(LandManager land, Vector2 spawnIntervalRange, float duration, int spawnAmount = 1)
    {
        if (land == null) return;
        if (land.EnemySpawner == null) return;

        EnemySpawner enemySpawner = land.EnemySpawner;
        enemySpawner.StartSpawnerWithDuration(spawnIntervalRange, duration, spawnAmount);
        activeSpawnerLands.Add(land);
    }

    /// <summary>
    /// Spawns enemies with currency on the specified lands.
    /// Adds the lands to the activeSpawnerLands list for tracking.
    /// </summary>
    /// <param name="lands">The list of lands to spawn enemies on.</param>
    /// <param name="willRestockCurrency">Whether to restock currency.</param>
    public void StartEnemySpawnersWithCurrency(List<LandManager> lands, Vector2 spawnIntervalRange, int spawnAmount = 1, bool willRestockCurrency = true)
    {
        foreach (LandManager land in new List<LandManager>(lands))
        {
            StartEnemySpawnerWithCurrency(land, spawnIntervalRange, spawnAmount, willRestockCurrency);
        }
    }

    /// <summary>
    /// Spawns enemies with a specified duration on the specified lands.
    /// Populates the enemySpawningCoroutines list with the coroutines from each land's enemy spawner.
    /// Adds the lands to the activeSpawnerLands list for tracking.
    /// </summary>
    /// /// <param name="lands">The list of lands to spawn enemies on.</param>
    /// /// <param name="duration">The duration of how long the enemies will spawn for.</param>
    public void StartEnemySpawnersWithDuration(List<LandManager> lands, Vector2 spawnIntervalRange, float duration, int spawnAmount = 1)
    {
        foreach (LandManager land in new List<LandManager>(lands))
        {
            StartEnemySpawnerWithDuration(land, spawnIntervalRange, duration, spawnAmount);
        }
    }

    /// <summary>
    /// Stops land from spawning enemies and removes it from the active lands list.
    /// </summary>
    /// <param name="land">The land to stop spawning.</param>
    public void StopEnemySpawner(LandManager land)
    {
        if (land == null) return;
        if (land.EnemySpawner == null) return;

        land.EnemySpawner.StopSpawner();
        activeSpawnerLands.Remove(land);
    }

    /// <summary>
    /// Stops and clears all active spawner lands.
    /// </summary>
    public void StopActiveEnemySpawners()
    {
        foreach (LandManager land in new List<LandManager>(activeSpawnerLands))
        {
            if (land == null) continue;
            if (land.EnemySpawner == null) continue;
            land.EnemySpawner.StopSpawner();
        }
        activeSpawnerLands.Clear();
    }

    /// <summary>
    /// Stops and clears all enemy spawners regardless of them being active.
    /// </summary>
    public void StopAllEnemySpawners()
    {
        foreach(LandManager land in new List<LandManager>(worldManager.SpawnedLands.Values))
        {
            if (land == null) continue;
            if (land.EnemySpawner == null) continue;
            land.EnemySpawner.StopSpawner();
        }
    }

    /// <summary>
    /// Updates the event UI elements. Called by EventProgressionUI script.
    /// </summary>
    /// <param name="feedbackText"></param>
    /// <param name="nameText"></param>
    /// <param name="optionalDescriptionText"></param>
    public abstract void UpdateEventUIElements(TMP_Text feedbackText, TMP_Text nameText, TMP_Text optionalDescriptionText);

    /// <summary>
    /// Formats a float timer into mm:ss string
    /// </summary>
    /// <param name="timer">The float timer to format.</param>
    /// <returns>The formatted mm:ss string</returns>
    public static string GetFormattedFloatTimer(float timer)
    {
        // Convert to minutes and seconds
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        // Format as mm:ss
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Converts the first letter of a string to uppercase and the rest to lowercase.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string FirstLetterToUpperOthersLower(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Convert the first letter to uppercase and the rest to lowercase
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }
}
