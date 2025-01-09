public class FollowerAttackRecoverState : FollowerBaseState
{
    private float recoverTimer;

    public override void OnEnter()
    {
        follower.SetSpeedModifier(0f);

        recoverTimer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        follower.ApplyGravity();

        recoverTimer += follower.LocalDeltaTime;

        if (recoverTimer > follower.AttackRecoverDuration)
        {
            follower.ChangeState(follower.DefaultState);
            return;
        }
    }
}
