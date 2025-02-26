public abstract class SlimeBaseState : EnemyBaseState
{
    private protected Slime slime;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        slime = entity as Slime;
    }
}
