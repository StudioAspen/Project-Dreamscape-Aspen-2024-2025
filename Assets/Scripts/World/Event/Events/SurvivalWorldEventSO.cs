using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// All lands spawn enemies for a certain amount of time, if the player survives that amount of time trigger EOW
[CreateAssetMenu(fileName = "Survival World Event", menuName = "World Event/Survival")]
public class SurvivalWorldEventSO : WorldEventSO
{
    [field: Header("Config")]
    [field: SerializeField] public float SurvivalEventDuration { get; private set; } = 120f;

    public float RemainingTime { get; private set; }

    private protected override void OnStarted()
    {
        // Spawn enemies on all lands for the duration of the event
        StartEnemySpawnersWithDuration(worldManager.SpawnedLands.Values.ToList(), SurvivalEventDuration);

        RemainingTime = SurvivalEventDuration;
    }

    private protected override void OnCleared()
    {
        StopEnemySpawners();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.KillAll();
        }
    }

    public override void OnUpdate()
    {
        RemainingTime -= Time.deltaTime;

        if(RemainingTime <= 0f)
        {
            RemainingTime = 0f;
            eventManager.ClearEvent();
            return;
        }
    }
}