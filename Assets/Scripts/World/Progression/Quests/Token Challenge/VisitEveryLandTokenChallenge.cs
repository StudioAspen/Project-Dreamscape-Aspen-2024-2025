using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[CreateAssetMenu(fileName = "Achieve Chain Count Progression Quest", menuName = "World/Progression Quest/Visit Every Land During Wave Token Challenge")]
public class VisitEveryLandTokenChallenge : ProgressionQuestSO
{
    private ChainingSystem chainingSystem;

    [field: Header("Config")]
    [field: SerializeField] public int ChainGoal { get; private set; } = 15;

    private Dictionary<Vector2Int, GameObject> visitIndicatorsDictionary = new Dictionary<Vector2Int, GameObject>();

    private protected WorldManager worldManager;

    private List<Player> players = new List<Player>();

    private protected override void OnActivated()
    {
        // Find all players and if there are none, clear the event
        players = GameObject.FindObjectsByType<Player>(FindObjectsSortMode.None).ToList();
        if (players == null)
        {
            CleanUp();
            return;
        }
        if (players.Count <= 0)
        {
            CleanUp();
            return;
        }

        chainingSystem = FindObjectOfType<ChainingSystem>();
        if (chainingSystem == null)
        {
            CleanUp();
            return;
        }

        // create visit indicators on all lands and set ChainGoal
        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            ChainGoal++;
            visitIndicatorsDictionary.Add(land.GridPosition,
                CustomDebug.InstantiateTemporarySphere(land.transform.position + 5f * Vector3.up, 10f, Mathf.Infinity, new Color(1, 0, 0, 0.5f)));
        }
    }

    private protected override void OnCleanUp()
    {

    }

    private protected override void OnUpdate()
    {
        if (chainingSystem == null) return; ;

        if (chainingSystem.ChainCount >= ChainGoal)
        {
            Complete();
            return;
        }

        // Check if any player is on a land and remove the visit indicator and add to chain count
        for (int i = 0; i < players.Count; i++)
        {
            Vector2Int playerGridPosition = worldManager.GetGridPosition(players[i].transform.position);

            if (visitIndicatorsDictionary.ContainsKey(playerGridPosition))
            {
                chainingSystem.AddChain();
                GameObject.Destroy(visitIndicatorsDictionary[playerGridPosition]);
                visitIndicatorsDictionary.Remove(playerGridPosition);
            }
        }
    }
}
