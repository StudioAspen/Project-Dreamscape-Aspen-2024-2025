public abstract class ShielderBaseState : EnemyBaseState
{
    private protected Shielder shielder;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        shielder = entity as Shielder;
    }
}
