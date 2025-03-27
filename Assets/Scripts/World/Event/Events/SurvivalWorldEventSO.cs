using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// All lands spawn enemies for a certain amount of time, if the player survives that amount of time trigger EOW
[CreateAssetMenu(fileName = "Survival World Event", menuName = "World/World Event/Survival")]
public class SurvivalWorldEventSO : WorldEventSO
{
    [field: Header("Config")]
    
    /// <summary>
    /// The minimum time limit, in seconds, of the event. Default value: 40 seconds.
    /// </summary>
    [field: Tooltip("The minimum time limit, in seconds, of the event. Default value: 40 seconds")]
    [field: Range(5f, 60f)]
    [field: SerializeField] public float BaseTimeLimit { get; private set; } = 40f;

    [field: Tooltip("The time, in seconds, this event will increase by as players progress. Default value: 20 seconds")]
    [field: Range(5f, 60f)]
    [field: SerializeField] public float TimeIncrement { get; private set; } = 10f;

    [field: Space(5)]

    /// <summary>
    /// The base number of intervals that will elapse during a timed event. Default value: 5 intervals
    /// </summary>
    [field: Tooltip("The base number of intervals that will elapse during a timed event. Default value: 5 intervals")]
    [field: Range(1, 6)]
    [field: SerializeField] public int BaseIntervals { get; private set; } = 5;

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

      // Get all spawned lands on the map
      List<LandManager> spawnedLands = worldManager.SpawnedLands.Values.ToList();

      int spawnAmount = BaseSpawnAmount + players.Count - 1;

      // Calculate the time limit based the number of spawned lands, the Base Time Limit, and the Time Increment.
      float timeLimit = BaseTimeLimit + (Mathf.FloorToInt((spawnedLands.Count - 1) / 2) * TimeIncrement);
      
      float interval = timeLimit / BaseIntervals;
      // Spawn enemies on all lands for the duration of the event
      StartEnemySpawnersWithDuration(spawnedLands, new Vector2(interval, interval), timeLimit, spawnAmount);

      RemainingTime = timeLimit;
    }

    private protected override void OnCleared()
    {
        StopActiveEnemySpawners();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.DeactivateAllEnemies();
        }
    }

    private protected override void OnUpdate()
    {
        RemainingTime -= Time.deltaTime;

        if(RemainingTime <= 0f)
        {
            RemainingTime = 0f;
            eventManager.ClearEvent();
            return;
        }
    }

    public override void UpdateEventUIElements(TMP_Text feedbackText, TMP_Text nameText, TMP_Text optionalDescriptionText)
    {
        feedbackText.text = $"{GetFormattedFloatTimer(RemainingTime)}";
        nameText.text = $"{EventProgressionUIName.ToUpper()}";
    }
}