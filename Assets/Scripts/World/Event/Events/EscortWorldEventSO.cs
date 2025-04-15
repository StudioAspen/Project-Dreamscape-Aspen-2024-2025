using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// An NPC will run around the map for X minutes.
// Only land the NPC stands on spawn enemies,if they survive trigger EOW
[CreateAssetMenu(fileName = "Escort World Event", menuName = "World/World Event/Escort")]
public class EscortWorldEventSO : WorldEventSO
{
    [field: Header("Config")]
    [field: SerializeField] public int EscortEventMaxHealth { get; private set; } = 200;
    [field: SerializeField] public EscortEventEntity EscortEventEntityPrefab { get; private set; }
    
    [field: Space(5)]

    /// <summary>
    /// The time that must elapse between spawning enemies during non-timed events. Default value: 8.0 seconds.
    /// </summary>
    [field: Tooltip("The time that must elapse between spawning enemies during non-time events. Default value: 8.0 seconds")]
    [field: Range(3f, 30f)]
    [field: SerializeField] public float BaseSpawnInterval { get; private set; } = 8f;

    public EscortEventEntity EscortEventEntity { get; private set; }
    private LandManager escortPreviousLand;
    private List<Player> players = new List<Player>();

    public float RemainingTime { get; private set; }

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

      // Spawn and Initialize the Escort Entity at the specified land.
      EscortEventEntity = GameObject.Instantiate(EscortEventEntityPrefab, land.EnemySpawner.GetRandomEnemySpawnPoint(1f), Quaternion.identity, eventManager.transform);
      EscortEventEntity.SetBaseMaxHealth(EscortEventMaxHealth, true);
      escortPreviousLand = land;

      land.EnemySpawner.MaterializeEntity(EscortEventEntity);

      // The land the escort entity spawns on will spawn enemies
      StartEnemySpawnerWithCurrency(land, new Vector2(BaseSpawnInterval, BaseSpawnInterval), BaseSpawnAmount);

      // Get a destination land. 
      Vector2Int originPosition = land.GridPosition;
      LandManager destinationLand = worldManager.GetFarthestLand(originPosition);

      




      // TODO: We need the Escort Entity to walk towards the destination land. Then we need to Trigger EOW when the entity reaches the destination land. We also would need an indicator like a debug sphere at the destination land.






      // Listen for the escort entity's death
      EscortEventEntity.OnEntityDeath += EscortEventEntity_OnEntityDeath;
    }

    private protected override void OnCleared()
    {
        StopActiveEnemySpawners();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.DeactivateAllEnemies();
        }

        // Remove the escort entity and cleanup the listener
        if(EscortEventEntity != null)
        {
            EscortEventEntity.OnEntityDeath -= EscortEventEntity_OnEntityDeath;
            GameObject.Destroy(EscortEventEntity.gameObject);
        }
    }

    private protected override void OnUpdate()
    {
        if (EscortEventEntity == null) return;

        MonitorEscortEntityLand();

        RemainingTime -= Time.deltaTime;

        if (RemainingTime <= 0 && EscortEventEntity.CurrentHealth > 0)
        {
            RemainingTime = 0f;
            eventManager.ClearEvent();
            return;
        }
    }

    /// <summary>
    /// Fired once when the escort entity changes land.
    /// </summary>
    /// <param name="newLand">The new land the escort entity has moved to.</param>
    private void OnEscortEntityChangeLand(LandManager newLand)
    {
        StopActiveEnemySpawners();

        StartEnemySpawnerWithCurrency(newLand, new Vector2(BaseSpawnInterval, BaseSpawnInterval), BaseSpawnAmount, false);
    }

    private void EscortEventEntity_OnEntityDeath(GameObject killerObject)
    {
        EscortEventEntity.OnEntityDeath -= EscortEventEntity_OnEntityDeath;
        
        StopActiveEnemySpawners();

        eventManager.FailEvent();
    }

    /// <summary>
    /// Monitors the land of the escort entity and triggers an event when it changes land.
    /// </summary>
    private void MonitorEscortEntityLand()
    {
        if (worldManager.TryGetLandByWorldPosition(EscortEventEntity.transform.position, out LandManager currentLand))
        {
            if (currentLand != escortPreviousLand)
            {
                OnEscortEntityChangeLand(currentLand);
                escortPreviousLand = currentLand;
            }
        }
    }

    public override void UpdateEventUIElements(TMP_Text feedbackText, TMP_Text nameText, TMP_Text optionalDescriptionText)
    {
        feedbackText.text = $"{GetFormattedFloatTimer(RemainingTime)}";
        nameText.text = $"{EventProgressionUIName.ToUpper()}";
    }
}
