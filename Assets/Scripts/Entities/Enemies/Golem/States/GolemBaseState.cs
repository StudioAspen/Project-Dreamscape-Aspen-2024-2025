public abstract class GolemBaseState : EnemyBaseState
{
    private protected Golem golem;

    public override void Init(Entity entity)
    {
        base.Init(entity);
        golem = entity as Golem;
    }
}