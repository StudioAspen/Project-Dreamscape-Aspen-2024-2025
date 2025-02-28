using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Achieve X Enemies Defeated Progression Quest", menuName = "World/Progression Quest/Achieve X Enemies Defeated")]
public class AchieveXEnemiesDefeatedProgressionQuestSO : ProgressionQuestSO
{
    private Player player;

    [field: Header("Config")]
    [field: SerializeField] public int DefeatedGoal { get; private set; } = 10;

    private int killCount;
    private protected override void OnActivated()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            OnCleanUp();
            return;
        }
        player.OnKillEntity += Player_OnKillEntity;
        killCount = 0;
    }

    private void Player_OnKillEntity(Entity entity)
    {
        killCount++;
        if (killCount >= DefeatedGoal)
        {
            Complete();
            return;
        }
    }

    private protected override void OnCleanUp()
    {
        player.OnKillEntity -= Player_OnKillEntity;
    }

    private protected override void OnUpdate()
    {

    }
}