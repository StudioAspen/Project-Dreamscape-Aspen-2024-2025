using UnityEngine;

public class FollowerWanderState : FollowerBaseState
{
    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    public override void OnEnter()
    {
        enemy.TransitionToAnimation("FlatMovement");

        follower.SetSpeedModifier(1f);

        wanderTimeElapsed = Mathf.Infinity;
        randomWanderIntervalDuration = Random.Range(follower.WanderIntervalDurationRange.x, follower.WanderIntervalDurationRange.y);
    }

    public override void OnExit()
    {
        follower.CancelPath();
    }

    public override void OnUpdate()
    {
        follower.ApplyGravity();

        wanderTimeElapsed += follower.LocalDeltaTime;

        if (wanderTimeElapsed > randomWanderIntervalDuration)
        {
            wanderTimeElapsed = 0f;
            randomWanderIntervalDuration = Random.Range(follower.WanderIntervalDurationRange.x, follower.WanderIntervalDurationRange.y);

            currentWanderDestination = follower.GetRandomWanderPoint(follower.WanderRadiusRange);
            follower.SetDestination(currentWanderDestination);
        }

        follower.MoveTowardsDestination();
        follower.SetSpeedModifier(follower.CloseToPoint(currentWanderDestination) ? 0f : 1f);

        if (follower.Target != null)
        {
            follower.ChangeState(follower.EnemyChaseState);
            return;
        }
    }
}
