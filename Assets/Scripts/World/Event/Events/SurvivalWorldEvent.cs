using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// All lands spawn enemies for a certain amount of time, if the player survives that amount of time trigger EOW
public class SurvivalWorldEvent : WorldEvent
{
    private TMP_Text UIText;

    private float remainingTime;

    public SurvivalWorldEvent(EventManager eventManager, WorldManager worldManager, EventsConfigSO eventsConfigSO) : base(eventManager, worldManager, eventsConfigSO) { }

    public override void OnStarted()
    {
        UIText = GameObject.Instantiate(eventsConfigSO.SurvivalEventUIPrefab, GameObject.FindGameObjectWithTag("Main Canvas").transform);

        // Spawn enemies on all lands for the duration of the event
        StartEnemySpawnersWithDuration(worldManager.SpawnedLands.Values.ToList(), eventsConfigSO.SurvivalEventDuration);

        remainingTime = eventsConfigSO.SurvivalEventDuration;
    }

    public override void OnCleared()
    {
        StopEnemySpawners();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.KillAll();
        }

        GameObject.Destroy(UIText.gameObject);
    }

    public override void Update()
    {
        remainingTime -= Time.deltaTime;

        UIText.text = $"{Mathf.Round(remainingTime)}s";

        if(remainingTime <= 0f)
        {
            remainingTime = 0f;
            eventManager.ClearEvent();
            return;
        }
    }
}