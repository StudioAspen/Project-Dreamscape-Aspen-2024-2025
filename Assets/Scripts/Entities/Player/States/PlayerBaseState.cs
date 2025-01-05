using UnityEngine;

public class PlayerBaseState : EntityBaseStateSO
{
    private protected Player player;

    public PlayerBaseState(Player player)
    {
        this.player = player;
    }
}
