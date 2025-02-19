using UnityEngine;

[System.Serializable]
public class FollowerWanderState : FollowerBaseState
{
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(3f, 5f);

    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    public override void OnEnter()
    {
        enemy.PlayDefaultAnimation();

        follower.SetSpeedModifier(1f);

        wanderTimeElapsed = Mathf.Infinity;
        randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);
    }

    public override void OnExit()
    {
        follower.CancelPath();
    }

    public override void OnUpdate()
    {
        follower.ApplyGravity();

        wanderTimeElapsed += follower.LocalDeltaTime;

        if(!follower.IsCurrentPathValid()) SetNewWanderDestination();

        if (wanderTimeElapsed > randomWanderIntervalDuration) SetNewWanderDestination();

        follower.MoveTowardsDestination();
        follower.SetSpeedModifier(follower.CloseToPoint(currentWanderDestination) ? 0f : 1f);

        if (follower.Target != null)
        {
            follower.ChangeState(follower.FollowerChaseState);
            return;
        }
    }

    private void SetNewWanderDestination()
    {
        wanderTimeElapsed = 0f;
        randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);

        currentWanderDestination = follower.GetRandomWanderPoint(WanderRadiusRange);
        follower.SetDestination(currentWanderDestination);
    }
}
