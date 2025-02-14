using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Achieve Momentum Count Progression Quest", menuName = "World/Progression Quest/Achieve Momentum Count")]
public class AchieveMomentumCountProgressionQuestSO : ProgressionQuestSO
{
    private List<MomentumSystem> momentumSystems = new();

    [field: Header("Config")]
    [field: SerializeField] public int MomentumGoal { get; private set; } = 5;

    private protected override void OnActivated()
    {
        momentumSystems = FindObjectsByType<MomentumSystem>(FindObjectsSortMode.None).ToList();
        if (momentumSystems == null)
        {
            CleanUp();
            return;
        }
        if (momentumSystems.Count == 0)
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
        foreach (MomentumSystem momentumSystem in momentumSystems)
        {
            if (momentumSystem == null) continue;

            if (momentumSystem.Momentum >= MomentumGoal)
            {
                Complete();
                return;
            }
        }
    }
}
