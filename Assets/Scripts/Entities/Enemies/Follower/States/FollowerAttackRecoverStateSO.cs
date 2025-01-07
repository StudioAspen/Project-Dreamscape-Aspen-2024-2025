public class FollowerAttackRecoverStateSO : FollowerBaseStateSO
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

    public override void Update()
    {
        follower.ApplyGravity();

        recoverTimer += follower.LocalDeltaTime;

        if (recoverTimer > follower.AttackRecoverDuration)
        {
            follower.ChangeState(follower.DefaultState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        
    }
}
