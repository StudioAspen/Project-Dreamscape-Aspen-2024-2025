using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Cross Between 4 Lands Progression Quest", menuName = "World/Progression Quest/Cross Between 4 Lands")]
public class CrossBetween4LandsinXsecondsProgressionQuestSO : ProgressionQuestSO
{
    private WorldManager worldManager;
    
    [field: Header("Config")]
    
    [field: SerializeField] public int timeGoal { get; private set; } = 15;

    private float timer;
    private bool questComplete = false;
    private HashSet<Vector2Int> visitedLands = new HashSet<Vector2Int>(); 
    private List<Player> players = new List<Player>();
    private protected override void OnActivated()
    {
        worldManager= FindObjectOfType<WorldManager>();
        if(worldManager == null)
        {
            CleanUp();
            return;
        }
        
        // Find all players (from VisitAllWorldEventSO)
        players = new List<Player>(GameObject.FindObjectsByType<Player>(FindObjectsSortMode.None));

        if (players == null || players.Count <= 0)
        {
            CleanUp();
            return;
        }
        
        timer = 0f;
        visitedLands.Clear();
        questComplete = false; 
    }

    private protected override void OnCleanUp()
    {
        
    }

    private protected override void OnUpdate()
    {
        if (worldManager == null || questComplete) return;

        timer += Time.deltaTime;

        // If the time has exceeded the time goal, fail the quest
        if (timer > timeGoal)
        {
            FailQuest(); 
            return;
        }
        
        // Check if any player has crossed a land
        foreach (Player player in players)
        {
            Vector2Int playerGridPosition = worldManager.GetGridPosition(player.transform.position);

            // If the player has crossed a land and hasn't visited it yet
            if (!visitedLands.Contains(playerGridPosition) && worldManager.SpawnedLands.ContainsKey(playerGridPosition))
            {
                visitedLands.Add(playerGridPosition);
            }
        }

        // Check if the player has visited 4 distinct lands
        if (visitedLands.Count >=4 )
        {
            CompleteQuest(); 
        }
    }
    private void CompleteQuest()
    {
        questComplete = true;
        //Debug.Log("Cross 4 lands challenge success!!");
        Complete(); 
    }

    // Fails the quest if time runs out
    private void FailQuest()
    {
        questComplete = true;
        //Debug.Log("Cross 4 lands challenge failed!!");
        
    }
}
