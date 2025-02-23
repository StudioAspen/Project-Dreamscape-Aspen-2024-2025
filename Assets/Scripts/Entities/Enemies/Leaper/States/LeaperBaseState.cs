public abstract class LeaperBaseState : EnemyBaseState
{
    private protected Leaper leaper;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        leaper = entity as Leaper;
    }
}
