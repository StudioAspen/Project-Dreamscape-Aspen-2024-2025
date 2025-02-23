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
    [field: SerializeField] public float DefendEventDuration { get; private set; } = 60f;
    [field: SerializeField] public int DefendEventMaxHealth { get; private set; } = 200;
    [field: SerializeField] public DefendEventEntity DefendEventEntityPrefab { get; private set; }
    public DefendEventEntity DefendEventEntity { get; private set; }

    public float RemainingTime { get; private set; }

    private protected override void OnStarted()
    {
        // All lands will spawn enemies
        StartEnemySpawnersWithCurrency(worldManager.SpawnedLands.Values.ToList());

        // Select a random land and spawn the defend event entity in the center of the land
        LandManager randomLand = worldManager.GetRandomLand();
        DefendEventEntity = GameObject.Instantiate(DefendEventEntityPrefab, randomLand.transform.position + 5f * Vector3.up, Quaternion.identity, eventManager.transform);
        DefendEventEntity.SetBaseMaxHealth(DefendEventMaxHealth, true);

        randomLand.EnemySpawner.MaterializeEntity(DefendEventEntity);

        // Listen for when the defend event entity dies
        DefendEventEntity.OnEntityDeath += DefendEventEntity_OnEntityDeath;

        RemainingTime = DefendEventDuration;
    }

    private protected override void OnCleared()
    {
        StopEnemySpawners();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.DeactivateAllEnemies();
        }
        
        // Remove the defend event entity and cleanup the listener
        if(DefendEventEntity != null)
        {
            DefendEventEntity.OnEntityDeath -= DefendEventEntity_OnEntityDeath;
            GameObject.Destroy(DefendEventEntity.gameObject);
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
