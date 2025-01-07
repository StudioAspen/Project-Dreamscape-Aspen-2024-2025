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
    [field: SerializeField] public TMP_Text EscortEventUIPrefab { get; private set; }

    private TMP_Text UIText;

    private EscortEventEntity escortEventEntity;
    private LandManager escortPreviousLand;

    float remainingTime;

    public override void OnStarted()
    {
        UIText = GameObject.Instantiate(EscortEventUIPrefab, GameObject.FindGameObjectWithTag("Main Canvas").transform);

        LandManager randomLand = worldManager.GetRandomLand();

        // Spawn the escort entity on the random land
        escortEventEntity = GameObject.Instantiate(EscortEventEntityPrefab, randomLand.transform.position + 6f * Vector3.up, Quaternion.identity, eventManager.transform);
        escortEventEntity.SetMaxHealth(EscortEventMaxHealth, true);
        escortPreviousLand = randomLand;

        // The land the escort entity spawns on will spawn enemies
        StartEnemySpawnerWithCurrency(randomLand);

        // Listen for the escort entity's death
        escortEventEntity.OnEntityDeath += EscortEventEntity_OnEntityDeath;

        remainingTime = EscortEventDuration;
    }

    public override void OnCleared()
    {
        StopEnemySpawners();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.KillAll();
        }

        // Remove the escort entity and cleanup the listener
        if(escortEventEntity != null)
        {
            escortEventEntity.OnEntityDeath -= EscortEventEntity_OnEntityDeath;
            GameObject.Destroy(escortEventEntity.gameObject);
        }

        GameObject.Destroy(UIText.gameObject);
    }

    public override void Update()
    {
        if (escortEventEntity == null) return;

        MonitorEscortEntityLand();

        remainingTime -= Time.deltaTime;

        UIText.text = $"{Mathf.Round(remainingTime)}s";

        if (remainingTime <= 0 && escortEventEntity.CurrentHealth > 0)
        {
            remainingTime = 0f;
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

        StartEnemySpawnerWithCurrency(newLand);
    }

    private void EscortEventEntity_OnEntityDeath(GameObject killerObject)
    {
        escortEventEntity.OnEntityDeath -= EscortEventEntity_OnEntityDeath;
        
        StopEnemySpawners();

        Debug.Log("Escort Event Entity has died. You failed.");
    }

    /// <summary>
    /// Monitors the land of the escort entity and triggers an event when it changes land.
    /// </summary>
    private void MonitorEscortEntityLand()
    {
        if (worldManager.TryGetLandByWorldPosition(escortEventEntity.transform.position, out LandManager currentLand))
        {
            if (currentLand != escortPreviousLand)
            {
                OnEscortEntityChangeLand(currentLand);
                escortPreviousLand = currentLand;
            }
        }
    }
}
