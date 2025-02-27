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
    [field: SerializeField] public float DefaultDuration { get; private set; } = 40f;
    

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

      // All lands will spawn enemies, based on the event duration
      StartEnemySpawnersWithDuration(worldManager.SpawnedLands.Values.ToList(), DefaultDuration);

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

      RemainingTime = DefaultDuration;
    }

    private protected override void OnCleared()
    {
        StopEnemySpawners();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
          land.EnemySpawner.DeactivateAllEnemies();

        // Remove the Defend Entity and Clean up the listener
        if(DefendEventEntity != null)
        {
          DefendEventEntity.OnEntityDeath -= DefendEventEntity_OnEntityDeath;
          Destroy(DefendEventEntity.gameObject);
        }
    }

    public override void OnUpdate()
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

        StopEnemySpawners();

        eventManager.FailEvent();
    }
}
