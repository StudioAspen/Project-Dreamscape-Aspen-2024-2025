using UnityEngine;

public class FollowerReadyAttackState : EnemyBaseState
{
    private Follower follower;

    private float readyTimer;
    private float readyDuration;

    public FollowerReadyAttackState(Follower enemy) : base(enemy)
    {
        follower = enemy;
    }

    public override void OnEnter()
    {
        follower.TransitionToAnimation("Attack", follower.AttackReadyDuration);

        follower.SetSpeedModifier(0f);

        readyDuration = Random.Range(0.5f * follower.AttackReadyDuration, 1.25f * follower.AttackReadyDuration);
        readyTimer = 0f;
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        follower.TransitionToAnimation("Attack");

        readyTimer += follower.LocalDeltaTime;

        if (readyTimer > readyDuration)
        {
            follower.ChangeState(follower.FollowerAttackState);
            return;
        }
    }

    public override void FixedUpdate()
    {

    }
}