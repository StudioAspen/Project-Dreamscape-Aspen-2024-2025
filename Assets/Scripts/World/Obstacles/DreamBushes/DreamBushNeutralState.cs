using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamBushNeutralState : DreamBushBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float PlayerDetectionRadius { get; private set; } = 5f;
    private List<Player> playerList = new List<Player>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, PlayerDetectionRadius);
    }

    public override void OnEnter()
    {
        dreamBush.OnDamaged += DreamBush_OnDamaged;
    }

    public override void OnExit()
    {
        dreamBush.OnDamaged -= DreamBush_OnDamaged;
        foreach (Player player in playerList)
        {
            player.OnEntityHeal -= Player_OnEntityHeal;
        }
        playerList.Clear();
    }

    public override void OnUpdate()
    {
        CheckForNearbyPlayers();
    }

    private void CheckForNearbyPlayers()
    {
        List<Entity> entityList = Entity.GetEntitiesThroughAOE(dreamBush.transform.position, PlayerDetectionRadius, false);
        List<Player> localPlayerList = new List<Player>();
        foreach (Entity entity in entityList)
        {
            Player player = entity as Player;
            if (player == null) continue;
            if (localPlayerList.Contains(player)) continue;
            localPlayerList.Add(player);
        }
        foreach (Player player in localPlayerList)
        {
            if (playerList.Contains(player)) continue;
            playerList.Add(player);
            OnPlayerEnter(player);
        }
        foreach (Player player in new List<Player>(playerList))
        {
            if (!localPlayerList.Contains(player))
            {
                playerList.Remove(player);
                OnPlayerExit(player);
            }
        }
    }

    private void OnPlayerEnter(Player player)
    {
        player.OnEntityHeal += Player_OnEntityHeal;
        Debug.Log($"{player.name} is nearby");
    }

    private void OnPlayerExit(Player player)
    {
        player.OnEntityHeal -= Player_OnEntityHeal;
        Debug.Log($"{player.name} left");
    }

    private void Player_OnEntityHeal(Entity healedEntity, int healValue)
    {
        dreamBush.ChangeState(dreamBush.DreamBushFriendlyState);
    }

    private void DreamBush_OnDamaged(Obstacle damagedObstacle, Vector3 hitPoint, GameObject source)
    {
        //Debug.Log($"{dreamBush.gameObject.name} was hit at {hitPoint} by {source.name}");
        dreamBush.ChangeState(dreamBush.DreamBushHostileState);
    }
}
