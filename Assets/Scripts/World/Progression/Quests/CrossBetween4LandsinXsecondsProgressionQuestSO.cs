using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Cross Between 4 Lands Progression Quest", menuName = "World/Progression Quest/Cross Between 4 Lands")]
public class CrossBetween4LandsinXsecondsProgressionQuestSO : ProgressionQuestSO
{
    private WorldManager worldManager;
    
    [field: Header("Config")]
    
    [field: SerializeField] public int timeGoal { get; private set; } = 15;

    private int timer;
    private protected override void OnActivated()
    {
        worldManager= FindObjectOfType<WorldManager>();
        if(worldManager == null)
        {
            CleanUp();
            return;
        }
    }

    private protected override void OnCleanUp()
    {
        
    }

    private protected override void OnUpdate()
    {
        if (worldManager == null) return;;

        if (worldManager.SpawnedLands.Count >= 3 && timer <= timeGoal )
        {
            Complete();
            return;
        }
    }
}
