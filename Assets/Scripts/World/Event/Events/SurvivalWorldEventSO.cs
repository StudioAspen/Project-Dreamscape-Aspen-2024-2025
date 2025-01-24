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
    [field: SerializeField] public TMP_Text SurvivalEventUIPrefab { get; private set; }

    private TMP_Text UIText;

    private float remainingTime;

    public override void OnStarted()
    {
        UIText = GameObject.Instantiate(SurvivalEventUIPrefab, GameObject.FindGameObjectWithTag("Main Canvas").transform);

        // Spawn enemies on all lands for the duration of the event
        StartEnemySpawnersWithDuration(worldManager.SpawnedLands.Values.ToList(), SurvivalEventDuration);

        remainingTime = SurvivalEventDuration;
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

    public override void OnUpdate()
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