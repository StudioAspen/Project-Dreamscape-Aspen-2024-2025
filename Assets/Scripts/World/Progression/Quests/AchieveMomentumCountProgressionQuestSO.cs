using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Achieve Momentum Count Progression Quest", menuName = "World/Progression Quest/Achieve Momentum Count")]
public class AchieveMomentumCountProgressionQuestSO : ProgressionQuestSO
{
    private MomentumSystem momentumSystem;

    [field: Header("Config")]
    [field: SerializeField] public int MomentumGoal { get; private set; } = 5;

    private protected override void OnActivated()
    {
        momentumSystem = FindObjectOfType<MomentumSystem>();
        if (momentumSystem == null)
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
        if (momentumSystem == null) return;

        if (momentumSystem.Momentum >= MomentumGoal)
        {
            Complete();
            return;
        }
    }
}
