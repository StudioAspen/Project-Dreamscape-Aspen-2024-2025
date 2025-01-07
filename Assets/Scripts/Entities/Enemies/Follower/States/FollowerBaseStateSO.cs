public class FollowerBaseStateSO : EnemyBaseStateSO
{
    private protected Follower follower;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);
        follower = entity as Follower;
    }
}
