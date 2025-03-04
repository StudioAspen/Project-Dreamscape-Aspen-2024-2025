using UnityEngine;

public abstract class EnemyBaseState : EntityBaseState
{
    private protected Enemy enemy;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        enemy = entity as Enemy;
    }
}