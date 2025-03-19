using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// A stationary object will placed at the center of the land for 1 minute, Every 30 seconds it will go to a neighboring land.
// All lands will spawn enemies, if the object survives, trigger EOW
[CreateAssetMenu(fileName = "Defend World Event", menuName = "World/World Event/Defend")]
public class DefendWorldEventSO : WorldEventSO
{
    [field: Header("Config")]
    
    /// <summary>
    /// The minimum time limit, in seconds, of the event. Default value: 40 seconds.
    /// </summary>
    [field: Tooltip("The minimum time limit, in seconds, of the event. Default value: 40 seconds")]
    [field: Range(5f, 60f)]
    [field: SerializeField] public float BaseTimeLimit { get; private set; } = 40f;

    [field: Tooltip("The time, in seconds, this event will increase by as players progress. Default value: 20 seconds")]
    [field: Range(5f, 60f)]
    [field: SerializeField] public float TimeIncrement { get; private set; } = 20f;

    [field:Space(5)]

    /// <summary>
    /// When this event selects lands to spawn enemies using Manhattan distance, the end layer is the index of the last layer searched. Default value: layer 2
    /// </summary>
    [field: Tooltip("When this event selects lands to spawn enemies using Manhattan distance, the end layer is the index of the last layer searched. Default value: layer 2")]
    [field: Range (0, 3)]
    [field: SerializeField] public int EndLayer { get; private set; } = 2;

    /// <summary>
    /// When this event selects lands to spawn enemies using Manhattan distance, decide whether to search the origin coordinate for a land. Default value: true
    /// </summary>
    [field: Tooltip("When this event selects lands to spawn enemies using Manhattan distance, decide whether to search the origin coordinate for a land. Default value: true")]
    [field: SerializeField] public bool CheckLayer0 { get; private set; } = true;

    [field: Space(5)]

    /// <summary>
    /// The base number of intervals that will elapse during a timed event. Default value: 3 intervals
    /// </summary>
    [field: Tooltip("The base number of intervals that will elapse during a timed event. Default value: 3 intervals")]
    [field: Range(1, 6)]
    [field: SerializeField] public int BaseIntervals { get; private set; } = 3;

    

    [field: Header("Defend Entity")]
    [field: SerializeField] public DefendEventEntity DefendEventEntityPrefab { get; private set; }
    [field: SerializeField] public int DefendEventMaxHealth { get; private set; } = 200;
    public DefendEventEntity DefendEventEntity { get; private set; }

    public float RemainingTime { get; private set; }

    private List<Player> players = new List<Player>();

    private protected override void OnStarted()
    {
      // Find all players and if there are none, clear the event
      players = FindObjectsByType<Player>(FindObjectsSortMode.None).ToList();
      
      if(players == null || players.Count <= 0)
      {
        eventManager.ClearEvent();
        return;
      }

      // Select a random player. Will always return players[0] if single player
      int randomIndex = UnityEngine.Random.Range(0, players.Count);
      Player randomPlayer = players.ElementAt(randomIndex);

      // Get the random player's position and find the land they're standing on
      Vector2Int playerGridPosition = worldManager.GetGridPosition(randomPlayer.transform.position);
      LandManager land = worldManager.GetLandByGridPosition(playerGridPosition);

      // Spawn and Initialize the Defend Entity at the specified land.
      DefendEventEntity = Instantiate(DefendEventEntityPrefab, land.transform.position + 5f * Vector3.up, Quaternion.identity, eventManager.transform);
      DefendEventEntity.SetBaseMaxHealth(DefendEventMaxHealth, true);
      land.EnemySpawner.MaterializeEntity(DefendEventEntity);

      // Listen for when the defend event entity dies
      DefendEventEntity.OnEntityDeath += DefendEventEntity_OnEntityDeath;

      // Use Manhattan distance to search 2 layers out from the Defend Entity's Land and make those lands spawn enemies.
      List<LandManager> activeLands = worldManager.GetLandsWithManhattanDistance(land.GridPosition, EndLayer, true);

      // Calculate the number of spawns per interval, based on the Base Spawn Amount and the number of players.
      int spawnAmount = BaseSpawnAmount + players.Count - 1;

      // Calculate the time limit based the number of active lands, the Base Time Limit, the Time Increment, and the End Layer.
      float timeLimit = BaseTimeLimit + (Mathf.FloorToInt((activeLands.Count - 1) / (EndLayer + 1)) * TimeIncrement);

      float interval = timeLimit / BaseIntervals;
      StartEnemySpawnersWithDuration(activeLands, new Vector2(interval, interval), timeLimit, spawnAmount);

      RemainingTime = timeLimit;
    }

    private protected override void OnCleared()
    {
        StopActiveEnemySpawners();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
          land.EnemySpawner.DeactivateAllEnemies();

        // Remove the Defend Entity and Clean up the listener
        if(DefendEventEntity != null)
        {
          DefendEventEntity.OnEntityDeath -= DefendEventEntity_OnEntityDeath;
          Destroy(DefendEventEntity.gameObject);
        }
    }

    private protected override void OnUpdate()
    {
        if(DefendEventEntity == null) return;

        RemainingTime -= Time.deltaTime;   

        if (RemainingTime <= 0 && DefendEventEntity.CurrentHealth > 0)
        {
            RemainingTime = 0f;
            eventManager.ClearEvent();
            return;
        }
    }

    private void DefendEventEntity_OnEntityDeath(GameObject killerObject)
    {
        DefendEventEntity.OnEntityDeath -= DefendEventEntity_OnEntityDeath;

        StopActiveEnemySpawners();

        eventManager.FailEvent();
    }

    public override void UpdateEventUIElements(TMP_Text feedbackText, TMP_Text nameText, TMP_Text optionalDescriptionText)
    {
        feedbackText.text = $"{GetFormattedFloatTimer(RemainingTime)}";
        nameText.text = $"{EventProgressionUIName.ToUpper()}";
    }
}
