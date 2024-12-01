using UnityEngine;

public class PlayerBaseState : EntityBaseState
{
    private protected Player player;

    public PlayerBaseState(Player player)
    {
        this.player = player;
    }
}
