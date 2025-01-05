using UnityEngine;

public class EnemyBaseState : EntityBaseStateSO
{
    private protected Enemy enemy;

    public EnemyBaseState(Enemy enemy)
    {
        this.enemy = enemy;
    }
}