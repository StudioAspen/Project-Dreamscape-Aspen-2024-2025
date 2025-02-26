using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Achieve Chain Count Progression Quest", menuName = "World/Progression Quest/Achieve Chain Count")]
public class AchieveChainCountProgressionQuestSO : ProgressionQuestSO
{
    private ChainingSystem chainingSystem;

    [field: Header("Config")]
    [field: SerializeField] public int ChainGoal { get; private set; } = 15;

    private protected override void OnActivated()
    {
        chainingSystem = FindObjectOfType<ChainingSystem>();
        if(chainingSystem == null)
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
        if (chainingSystem == null) return;;

        if (chainingSystem.ChainCount >= ChainGoal)
        {
            Complete();
            return;
        }
    }
}
