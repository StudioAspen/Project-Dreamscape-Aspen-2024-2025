public abstract class EnemyBaseState : BaseState
{
    private protected Enemy enemy;

    public EnemyBaseState(Enemy enemy)
    {
        this.enemy = enemy;
    }
}