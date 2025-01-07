using UnityEngine;

public class EnemyBaseStateSO : EntityBaseStateSO
{
    private protected Enemy enemy;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);
        enemy = entity as Enemy;
    }
}