public class SlimeBaseState : EnemyBaseState
{
    private protected Slime slime;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        slime = entity as Slime;
    }
}
