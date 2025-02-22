using UnityEngine;

public abstract class PlayerBaseState : EntityBaseState
{
    private protected Player player;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        player = entity as Player;
    }
}
