using UnityEngine;

public class EnemyBaseState : EntityBaseState
{
    private protected Enemy enemy;

    public EnemyBaseState(Enemy enemy)
    {
        this.enemy = enemy;
    }
}