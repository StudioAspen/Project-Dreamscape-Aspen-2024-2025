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
    [field: SerializeField] public int VisitAllEventDummyVariable { get; private set; }

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

        // Start enemy spawners and create visit indicators on all lands
        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            StartEnemySpawnerWithCurrency(land);

            visitIndicatorsDictionary.Add(land.GridPosition,
                CustomDebug.InstantiateTemporarySphere(land.transform.position + 5f * Vector3.up, 10f, Mathf.Infinity, new Color(1, 0, 0, 0.5f)));
        }
    }

    private protected override void OnCleared()
    {
        StopEnemySpawners();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.KillAll();
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

        // Check if any player is on a land and remove the visit indicator
        for (int i = 0; i < players.Count; i++)
        {
            Vector2Int playerGridPosition = worldManager.GetGridPosition(players[i].transform.position);

            if (visitIndicatorsDictionary.ContainsKey(playerGridPosition))
            {
                GameObject.Destroy(visitIndicatorsDictionary[playerGridPosition]);
                visitIndicatorsDictionary.Remove(playerGridPosition);
            }
        }
    }
}