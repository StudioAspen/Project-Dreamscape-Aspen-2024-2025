using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// A stationary object will placed at the center of the land for 1 minute, Every 30 seconds it will go to a neighboring land.
// All lands will spawn enemies, if the object survives, trigger EOW
public class DefendWorldEvent : BaseState
{
    private EventManager eventManager;
    private WorldManager worldManager;
    private EventsConfigSO eventsConfigSO;

    private TMP_Text UIText;

    private List<Coroutine> enemySpawningCoroutines = new List<Coroutine>();
    private DefendEventEntity defendEventEntity;

    float remainingTime;

    public DefendWorldEvent(EventManager eventManager, WorldManager worldManager, EventsConfigSO eventsConfigSO)
    {
        this.eventManager = eventManager;
        this.worldManager = worldManager;
        this.eventsConfigSO = eventsConfigSO;
    }

    public override void OnEnter()
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

    public override void OnExit()
    {
        foreach (Coroutine coroutine in enemySpawningCoroutines)
        {
            if (coroutine != null) eventManager.StopCoroutine(coroutine);
        }
        enemySpawningCoroutines.Clear();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.KillAll();
        }

        defendEventEntity.OnEntityDeath -= DefendEventEntity_OnEntityDeath;
        GameObject.Destroy(defendEventEntity.gameObject);

        GameObject.Destroy(UIText.gameObject);
    }

    public override void Update()
    {
        if(defendEventEntity != null) remainingTime -= Time.deltaTime;   

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
        Debug.Log("Defend Event Entity has died. You failed.");
    }
}
