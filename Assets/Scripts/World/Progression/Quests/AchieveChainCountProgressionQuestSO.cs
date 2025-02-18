using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Achieve Chain Count Progression Quest", menuName = "World/Progression Quest/Achieve Chain Count")]
public class AchieveChainCountProgressionQuestSO : ProgressionQuestSO
{
    private List<ChainingSystem> chainingSystems = new();

    [field: Header("Config")]
    [field: SerializeField] public int ChainGoal { get; private set; } = 15;

    private protected override void OnActivated()
    {
        chainingSystems = FindObjectsByType<ChainingSystem>(FindObjectsSortMode.None).ToList();
        if(chainingSystems == null)
        {
            CleanUp();
            return;
        }
        if(chainingSystems.Count == 0)
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
        foreach(ChainingSystem chainingSystem in chainingSystems)
        {
            if(chainingSystem == null) continue;

            if(chainingSystem.ChainCount >= ChainGoal)
            {
                Complete();
                return;
            }
        }
    }
}
