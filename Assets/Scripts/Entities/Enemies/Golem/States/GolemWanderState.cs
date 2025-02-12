using UnityEngine;

public class GolemWanderState : GolemBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(3f, 5f);

    private float wanderTimeElapsed;
    private float randomWanderIntervalDuration;
    private Vector3 currentWanderDestination;

    public override void OnEnter()
    {
        enemy.TransitionToAnimation("FlatMovement");

        golem.SetSpeedModifier(.5f);

        wanderTimeElapsed = Mathf.Infinity;
        randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);
    }

    public override void OnExit()
    {
        golem.CancelPath();
    }

    public override void OnUpdate()
    {
        golem.ApplyGravity();

        wanderTimeElapsed += golem.LocalDeltaTime;

        if(!golem.IsCurrentPathValid()) SetNewWanderDestination();

        if (wanderTimeElapsed > randomWanderIntervalDuration) SetNewWanderDestination();

        golem.MoveTowardsDestination();
        golem.SetSpeedModifier(golem.CloseToPoint(currentWanderDestination) ? 0f : 1f);

        if (golem.Target != null)
        {
            golem.ChangeState(golem.GolemChaseState);
            return;
        }
    }

    private void SetNewWanderDestination()
    {
        wanderTimeElapsed = 0f;
        randomWanderIntervalDuration = Random.Range(WanderIntervalDurationRange.x, WanderIntervalDurationRange.y);

        currentWanderDestination = golem.GetRandomWanderPoint(WanderRadiusRange);
        golem.SetDestination(currentWanderDestination);
    }
}
