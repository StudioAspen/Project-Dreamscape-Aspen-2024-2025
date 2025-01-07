using UnityEngine;

public class PlayerBaseStateSO : EntityBaseStateSO
{
    private protected Player player;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        player = entity as Player;
    }
}
