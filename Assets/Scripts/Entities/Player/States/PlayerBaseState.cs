using UnityEngine;

public class PlayerBaseState : EntityBaseState
{
    private protected Player player;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        player = entity as Player;
    }
}
