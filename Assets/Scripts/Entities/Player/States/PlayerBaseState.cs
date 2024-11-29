using UnityEngine;

public class PlayerBaseState : BaseState
{
    private protected Player player;

    public PlayerBaseState(Player player)
    {
        this.player = player;
    }
}
