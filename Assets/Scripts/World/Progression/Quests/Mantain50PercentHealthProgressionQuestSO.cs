using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Maintain 50 Percent Health Progression Quest", menuName = "World/Progression Quest/Maintain 50 Percent Health")]
public class Maintain50PercentHealthProgressionQuestSO : ProgressionQuestSO
{
    private Player player;
    private bool isFailed;

    private protected override void OnActivated()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            isFailed = true;
            CleanUp();
            return;
        }
    }

    private protected override void OnCleanUp()
    {
        if (isFailed) return;
        CompleteWithoutCleanUp();
    }

    private protected override void OnUpdate()
    {
        if (player == null) return;

        if (isFailed) return;

        if (player.CurrentHealth < player.MaxHealth.GetIntValue() / 2)
        {
            isFailed = true;
            return;
        }
    }
}

