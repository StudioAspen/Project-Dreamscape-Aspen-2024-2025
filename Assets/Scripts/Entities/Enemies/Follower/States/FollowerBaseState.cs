public abstract class FollowerBaseState : EnemyBaseState
{
    private protected Follower follower;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        follower = entity as Follower;
    }
}
