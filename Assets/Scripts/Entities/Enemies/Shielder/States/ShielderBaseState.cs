public class ShielderBaseState : EnemyBaseState
{
    private protected Shielder shielder;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        shielder = entity as Shielder;
    }
}
