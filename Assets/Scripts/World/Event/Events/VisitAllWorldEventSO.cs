using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// All lands will light up. When the player steps on a land it will go away. All lands will spawn enemies.
// Once all the lands have been touched by the player, trigger EOW
[CreateAssetMenu(fileName = "Visit All World Event", menuName = "World/World Event/Visit All")]
public class VisitAllWorldEventSO : WorldEventSO
{
    [field: Header("Indicator Count Settings")]
    // Previously named VisitAllEventDummyVarable, and previously declared as an int.

    /// <summary>
    /// Controls how many lands will be indicated for this event. The greater the value, the more lands will be indicated.
    /// </summary>
    [field: Range(1f, 15f)]
    [field: SerializeField] public float CountModifier { get; private set; }

    [field: Space(5)]

    [field: Header("Indicator Sphere Settings")]
    /// <summary>
    /// Controls the scale of a land's visit indicator sphere at land level -5
    /// </summary>
    [field: Range(3, 15)]
    [field: SerializeField] public float YIntercept { get; private set; }
    
    /// <summary>
    /// Controls a Indicator Sphere's rate of decaying scale as its land's level increases. 
    /// </summary>
    [field: Range(0f, 1f)]
    [field: SerializeField] public float RadiusDecayRate { get; private set; }

    /// <summary>
    /// Controls the minimum radius of a land's visit indicator sphere. 
    /// </summary>
    [field: Range(0, 3)]
    [field: SerializeField] public float MinimumRadius { get; private set; }

    [field: Header("Config")]
    /// <summary>
    /// The time that must elapse between spawning enemies during non-timed events. Default value: 3.0 seconds.
    /// </summary>
    [field: Tooltip("The time that must elapse between spawning enemies during non-time events. Default value: 3.0 seconds")]
    [field: Range(3f, 30f)]
    [field: SerializeField] public float BaseSpawnInterval { get; private set; } = 3f;

    private Player player;

    private Dictionary<Vector2Int, GameObject> visitIndicatorsDictionary = new Dictionary<Vector2Int, GameObject>();

    private int totalLands;

    private protected override void OnStarted()
    {
        visitIndicatorsDictionary = new();

        // Find player and if there are none, clear the event
        player = FindObjectOfType<Player>();
        if(player == null)
        {
            eventManager.ClearEvent();
            return;
        }

        // Equation returns a radical number <= the current spawned lands count. This is to balance the late game as the map gets larger.
        int landCount = worldManager.SpawnedLands.Count;
        int indicatorCount = Mathf.Clamp(Mathf.RoundToInt(Mathf.Sqrt(CountModifier * landCount)), 0, landCount);

        // Activate random lands until the dictionary meets the indicator count for the current wave
        while (visitIndicatorsDictionary.Count < indicatorCount) 
        {
          // Get a random index from worldManager.SpawnedLands
          LandManager land = worldManager.GetRandomLand();

          // If we already selected that land, try again
          if (visitIndicatorsDictionary.ContainsKey(land.GridPosition))
            continue;

          StartEnemySpawnerWithCurrency(land, new Vector2(BaseSpawnInterval, BaseSpawnInterval), BaseSpawnAmount);

          // Since we're using the land level as a power, we need to ensure it's always >= 0
          int absLandLevel = land.Level + 5;
          float sphereRadius = YIntercept * Mathf.Pow(1 - RadiusDecayRate, absLandLevel) + MinimumRadius;

          visitIndicatorsDictionary.Add(land.GridPosition, CustomDebug.InstantiateTemporarySphere(land.transform.position + 5f * Vector3.up, sphereRadius, Mathf.Infinity, new Color(1, 0, 0, 0.5f)));
        }

        Vector2Int playerGridPosition = worldManager.GetGridPosition(player.transform.position);
        // Automatically remove the visit Indicator of the land the player is standing on at the start of the event.
        if (visitIndicatorsDictionary.ContainsKey(playerGridPosition))
        {
            GameObject.Destroy(visitIndicatorsDictionary[playerGridPosition]);
            visitIndicatorsDictionary.Remove(playerGridPosition);
        }

        totalLands = visitIndicatorsDictionary.Count;
    }

    private protected override void OnCleared()
    {
        StopActiveEnemySpawners();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.DeactivateAllEnemies();
        }

        // Cleanup all visit indicators
        foreach (GameObject sphere in visitIndicatorsDictionary.Values)
        {
            GameObject.Destroy(sphere);
        }
        visitIndicatorsDictionary.Clear();
    }

    private protected override void OnUpdate()
    {
        if (visitIndicatorsDictionary.Count <= 0)
        {
            eventManager.ClearEvent();
            return;
        }

        Vector2Int playerGridPosition = worldManager.GetGridPosition(player.transform.position);
        if (visitIndicatorsDictionary.ContainsKey(playerGridPosition))
        {
            GameObject visitIndicator = visitIndicatorsDictionary[playerGridPosition];
            float sphereRadius = visitIndicator.transform.localScale.x / 2;

            //Debug.Log($"Land Level: {worldManager.GetLandByGridPosition(playerGridPosition).Level} \n Radius: {sphereRadius}");

            //Check if the player is within the visit indicator, and remove the visit indicator if so.
            if (Vector3.Distance(player.transform.position, visitIndicator.transform.position) <= sphereRadius)
            {
                GameObject.Destroy(visitIndicatorsDictionary[playerGridPosition]);
                visitIndicatorsDictionary.Remove(playerGridPosition);
            }
        }
    }

    public override void UpdateEventUIElements(TMP_Text feedbackText, TMP_Text nameText, TMP_Text optionalDescriptionText)
    {
        feedbackText.text = $"{totalLands - visitIndicatorsDictionary.Count}/{totalLands}";
        nameText.text = $"{EventProgressionUIName.ToUpper()}";
    }
}