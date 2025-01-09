using UnityEngine;

public class FollowerReadyAttackState : FollowerBaseState
{
    private float readyTimer;
    private float readyDuration;

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

    public override void OnUpdate()
    {
        follower.ApplyGravity();

        follower.TransitionToAnimation("Attack");

        readyTimer += follower.LocalDeltaTime;

        if (readyTimer > readyDuration)
        {
            follower.ChangeState(follower.FollowerAttackState);
            return;
        }
    }
}