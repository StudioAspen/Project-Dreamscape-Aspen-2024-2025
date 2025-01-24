public class LeaperBaseState : EnemyBaseState
{
    private protected Leaper leaper;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        leaper = entity as Leaper;
    }
}
