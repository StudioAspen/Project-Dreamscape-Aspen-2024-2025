using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// An NPC will run around the map for X minutes.
// Only land the NPC stands on spawn enemies,if they survive trigger EOW
[CreateAssetMenu(fileName = "Escort World Event", menuName = "World Event/Escort")]
public class EscortWorldEventSO : WorldEventSO
{
    [field: Header("Config")]
    [field: SerializeField] public float EscortEventDuration { get; private set; } = 120f;
    [field: SerializeField] public int EscortEventMaxHealth { get; private set; } = 200;
    [field: SerializeField] public EscortEventEntity EscortEventEntityPrefab { get; private set; }
    public EscortEventEntity EscortEventEntity { get; private set; }
    private LandManager escortPreviousLand;

    public float RemainingTime { get; private set; }

    private protected override void OnStarted()
    {
        LandManager randomLand = worldManager.GetRandomLand();

        // Spawn the escort entity on the random land
        EscortEventEntity = GameObject.Instantiate(EscortEventEntityPrefab, randomLand.transform.position + 6f * Vector3.up, Quaternion.identity, eventManager.transform);
        EscortEventEntity.SetMaxHealth(EscortEventMaxHealth, true);
        escortPreviousLand = randomLand;

        randomLand.EnemySpawner.MaterializeEntity(EscortEventEntity);

        // The land the escort entity spawns on will spawn enemies
        StartEnemySpawnerWithCurrency(randomLand);

        // Listen for the escort entity's death
        EscortEventEntity.OnEntityDeath += EscortEventEntity_OnEntityDeath;

        RemainingTime = EscortEventDuration;
    }

    private protected override void OnCleared()
    {
        StopEnemySpawners();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.KillAll();
        }

        // Remove the escort entity and cleanup the listener
        if(EscortEventEntity != null)
        {
            EscortEventEntity.OnEntityDeath -= EscortEventEntity_OnEntityDeath;
            GameObject.Destroy(EscortEventEntity.gameObject);
        }
    }

    public override void OnUpdate()
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
        StopEnemySpawners();

        StartEnemySpawnerWithCurrency(newLand, false);
    }

    private void EscortEventEntity_OnEntityDeath(GameObject killerObject)
    {
        EscortEventEntity.OnEntityDeath -= EscortEventEntity_OnEntityDeath;
        
        StopEnemySpawners();

        Debug.Log("Escort Event Entity has died. You failed.");
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
}
