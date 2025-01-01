using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// An NPC will run around the map for X minutes.
// Only land the NPC stands on spawn enemies,if they survive trigger EOW
public class EscortWorldEvent : WorldEvent
{
    private TMP_Text UIText;

    private EscortEventEntity escortEventEntity;
    private LandManager escortPreviousLand;

    float remainingTime;

    public EscortWorldEvent(EventManager eventManager, WorldManager worldManager, EventsConfigSO eventsConfigSO) : base(eventManager, worldManager, eventsConfigSO) { }

    public override void OnStarted()
    {
        UIText = GameObject.Instantiate(eventsConfigSO.EscortEventUIPrefab, GameObject.FindGameObjectWithTag("Main Canvas").transform);

        LandManager randomLand = worldManager.GetRandomLand();

        escortEventEntity = GameObject.Instantiate(eventsConfigSO.EscortEventEntityPrefab, randomLand.transform.position + 6f * Vector3.up, Quaternion.identity, eventManager.transform);
        escortEventEntity.SetMaxHealth(eventsConfigSO.EscortEventMaxHealth, true);
        escortPreviousLand = randomLand;

        enemySpawningCoroutines.Add(eventManager.StartCoroutine(randomLand.EnemySpawner.SpawnWithCurrencyCoroutine()));

        escortEventEntity.OnEntityDeath += EscortEventEntity_OnEntityDeath;

        remainingTime = eventsConfigSO.EscortEventDuration;
    }

    public override void OnCleared()
    {
        StopAndClearEnemySpawningCoroutines();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.KillAll();
        }

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
        StopAndClearEnemySpawningCoroutines();

        enemySpawningCoroutines.Add(eventManager.StartCoroutine(newLand.EnemySpawner.SpawnWithCurrencyCoroutine()));
    }

    private void EscortEventEntity_OnEntityDeath(GameObject killerObject)
    {
        escortEventEntity.OnEntityDeath -= EscortEventEntity_OnEntityDeath;
        
        StopAndClearEnemySpawningCoroutines();

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
