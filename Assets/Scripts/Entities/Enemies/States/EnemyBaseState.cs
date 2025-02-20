using UnityEngine;

public class EnemyBaseState : EntityBaseState
{
    private protected Enemy enemy;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        enemy = entity as Enemy;
    }
}