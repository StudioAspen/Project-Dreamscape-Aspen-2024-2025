using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// A stationary object will placed at the center of the land for 1 minute, Every 30 seconds it will go to a neighboring land.
// All lands will spawn enemies, if the object survives, trigger EOW
public class DefendWorldEvent : WorldEvent
{
    private TMP_Text UIText;

    private DefendEventEntity defendEventEntity;

    float remainingTime;

    public DefendWorldEvent(EventManager eventManager, WorldManager worldManager, EventsConfigSO eventsConfigSO) : base(eventManager, worldManager, eventsConfigSO) { }

    public override void OnStarted()
    {
        UIText = GameObject.Instantiate(eventsConfigSO.DefendEventUIPrefab, GameObject.FindGameObjectWithTag("Main Canvas").transform);

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            EnemySpawner enemySpawner = land.EnemySpawner;
            enemySpawningCoroutines.Add(eventManager.StartCoroutine(enemySpawner.SpawnWithCurrencyCoroutine()));
        }

        LandManager randomLand = worldManager.GetRandomLand();
        defendEventEntity = GameObject.Instantiate(eventsConfigSO.DefendEventEntityPrefab, randomLand.transform.position + 6f * Vector3.up, Quaternion.identity, eventManager.transform);
        defendEventEntity.SetMaxHealth(eventsConfigSO.DefendEventMaxHealth, true);

        defendEventEntity.OnEntityDeath += DefendEventEntity_OnEntityDeath;

        remainingTime = eventsConfigSO.DefendEventDuration;
    }

    public override void OnCleared()
    {
        StopAndClearEnemySpawningCoroutines();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.KillAll();
        }
        
        if(defendEventEntity != null)
        {
            defendEventEntity.OnEntityDeath -= DefendEventEntity_OnEntityDeath;
            GameObject.Destroy(defendEventEntity.gameObject);
        }

        GameObject.Destroy(UIText.gameObject);
    }

    public override void Update()
    {
        if(defendEventEntity == null) return;

        remainingTime -= Time.deltaTime;   

        UIText.text = $"{Mathf.Round(remainingTime)}s";

        if (remainingTime <= 0 && defendEventEntity.CurrentHealth > 0)
        {
            remainingTime = 0f;
            eventManager.ClearEvent();
            return;
        }
    }

    private void DefendEventEntity_OnEntityDeath(GameObject killerObject)
    {
        defendEventEntity.OnEntityDeath -= DefendEventEntity_OnEntityDeath;

        Debug.Log("Defend Event Entity has died. You failed.");
    }
}
