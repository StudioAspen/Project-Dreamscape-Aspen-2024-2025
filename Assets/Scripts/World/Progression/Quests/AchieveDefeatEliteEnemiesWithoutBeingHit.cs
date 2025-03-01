using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchieveDefeatEliteEnemiesWithoutBeingHit : ProgressionQuestSO
{
    private Player player;

    [field: Header("Config")]
    [field: SerializeField] public int DefeatedElite { get; private set; } = 10;

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
        
        player.OnEntityTakeDamage += player_OnTakeDamage;
    }

    private void player_OnTakeDamage(int damage, Vector3 hitPosition, GameObject source)
    {
            killCount = 0;
    }

    private void Player_OnKillEntity(Entity entity)
    {

        killCount++;
        if (killCount >= DefeatedElite)
        {
            Complete();
            return;
        }
    }

    private protected override void OnCleanUp()
    {
        player.OnKillEntity -= Player_OnKillEntity;
        player.OnEntityTakeDamage -= player_OnTakeDamage;
    }

    private protected override void OnUpdate()
    {

    }
}
