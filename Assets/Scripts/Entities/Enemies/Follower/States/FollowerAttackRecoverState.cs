using UnityEngine;

public class FollowerAttackRecoverState : EnemyBaseState
{
    private Follower follower;

    private float recoverTimer;

    public FollowerAttackRecoverState(Follower enemy) : base(enemy)
    {
        follower = enemy;
    }

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
        recoverTimer += Time.deltaTime;

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
