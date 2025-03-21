using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Visit All Progression Quest", menuName = "World/Progression Quest/Visit Every Land During Wave Token Challenge")]
public class VisitEveryLandTokenChallenge : ProgressionQuestSO
{
    private int currentLandVisited;
    private int landsNeeded;

    private Dictionary<Vector2Int, GameObject> visitIndicatorsDictionary = new Dictionary<Vector2Int, GameObject>();

    private protected WorldManager worldManager;

    private Player player;

    private protected override void OnActivated()
    {
        // find all players and if there are none, clear the event
        player = GameObject.FindObjectOfType<Player>();
        if (player == null)
        {
            CleanUp();
            return;
        }

        // create visit indicators on all lands and set ChainGoal
        worldManager = GameObject.FindObjectOfType<WorldManager>();

        if (worldManager == null)
        {
            CleanUp();
            return;
        }

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            landsNeeded++;
            visitIndicatorsDictionary.Add(land.GridPosition,
                CustomDebug.InstantiateTemporarySphere(land.transform.position + 5f * Vector3.up, 10f, Mathf.Infinity, new Color(1, 0, 0, 0.5f)));
        }
    }

    private protected override void OnCleanUp()
    {
        foreach(GameObject indicator in visitIndicatorsDictionary.Values)
        {
            GameObject.Destroy(indicator);
        }
        visitIndicatorsDictionary.Clear();
    }

    private protected override void OnUpdate()
    {
        if(currentLandVisited == landsNeeded)
        {
            this.Complete();
            return;
        }


        // Check if any player is on a land and remove the visit indicator and add to chain count
        Vector2Int playerGridPosition = worldManager.GetGridPosition(player.transform.position);

        if (visitIndicatorsDictionary.ContainsKey(playerGridPosition))
        {
            currentLandVisited++;
            GameObject.Destroy(visitIndicatorsDictionary[playerGridPosition]);
            visitIndicatorsDictionary.Remove(playerGridPosition);
        }
    }
}
