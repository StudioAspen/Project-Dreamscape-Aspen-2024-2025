using UnityEngine;

public class PlayerBaseState : EntityBaseStateSO
{
    private protected Player player;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        player = entity as Player;
    }
}
