using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// All lands will light up. When the player steps on a land it will go away. All lands will spawn enemies.
// Once all the lands have been touched by the player, trigger EOW
[CreateAssetMenu(fileName = "Visit All World Event", menuName = "World Event/Visit All")]
public class VisitAllWorldEventSO : WorldEventSO
{
    [field: Header("Config")]
    // Previously named VisitAllEventDummyVarable, and previously declared as an int.

    /// <summary>
    /// Controls how many lands will be indicated for this event. The greater the value, the more lands will be indicated.
    /// </summary>
    [field: Range(1f, 15f)]
    [field: SerializeField] public float CountModifier { get; private set; }

    /// <summary>
    /// Controls the radius of each Visit Indicator sphere.
    /// </summary>
    [field: Range(2.5f, 15f)]
    [field: SerializeField] public float VisitIndicatorsRadius { get; private set; }

    private List<Player> players = new List<Player>();

    private Dictionary<Vector2Int, GameObject> visitIndicatorsDictionary = new Dictionary<Vector2Int, GameObject>();

    private protected override void OnStarted()
    {
        // Find all players and if there are none, clear the event
        players = GameObject.FindObjectsByType<Player>(FindObjectsSortMode.None).ToList();
        if(players == null)
        {
            eventManager.ClearEvent();
            return;
        }
        if(players.Count <= 0)
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

          StartEnemySpawnerWithCurrency(land);

          visitIndicatorsDictionary.Add(land.GridPosition, CustomDebug.InstantiateTemporarySphere(land.transform.position + 5f * Vector3.up, VisitIndicatorsRadius, Mathf.Infinity, new Color(1, 0, 0, 0.5f)));
        }

        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            Vector2Int playerGridPosition = worldManager.GetGridPosition(player.transform.position);

            // Automatically remove the visit Indicator of the land the player is standing on at the start of the event.
            if (visitIndicatorsDictionary.ContainsKey(playerGridPosition))
            {
                GameObject.Destroy(visitIndicatorsDictionary[playerGridPosition]);
                visitIndicatorsDictionary.Remove(playerGridPosition);
            }   
        }
    }

    private protected override void OnCleared()
    {
        StopEnemySpawners();

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

    public override void OnUpdate()
    {
        if (visitIndicatorsDictionary.Count <= 0)
        {
            eventManager.ClearEvent();
            return;
        }

        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            Vector2Int playerGridPosition = worldManager.GetGridPosition(player.transform.position);

            if (visitIndicatorsDictionary.ContainsKey(playerGridPosition))
            {
                GameObject visitIndicator = visitIndicatorsDictionary[playerGridPosition];

                //Check if the player is within the visit indicator, and remove the visit indicator if so.
                if (Vector3.Distance(player.transform.position, visitIndicator.transform.position) <= VisitIndicatorsRadius)
                {
                  GameObject.Destroy(visitIndicatorsDictionary[playerGridPosition]);
                  visitIndicatorsDictionary.Remove(playerGridPosition);
                }
            } 
        }
    }
}