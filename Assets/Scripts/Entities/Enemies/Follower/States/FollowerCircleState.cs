using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FollowerCircleState : FollowerBaseState
{
    [field: SerializeField] public int CircleFollowerCountThreshold { get; private set; } = 2;
    [field: SerializeField] public float ChangeDirectionInterval { get; private set; } = 0.5f;
    [field: SerializeField] public int ChangeDirectionReciprocal { get; private set; } = 50;
    [field: SerializeField] public float CircleRadius { get; private set; } = 5f;
    [field: SerializeField] public float MaxCircleRadius { get; private set; } = 8f;

    private bool cwCircle;

    private float changeDirTimer;

    private float canChaseTimer;
    private float canChaseCooldown = 3f;

    public override void OnEnter()
    {
        follower.PlayDefaultAnimation();

        follower.SetSpeedModifier(0.5f);

        Ticker.Instance.OnTick += Ticker_OnTick;

        changeDirTimer = 0f;

        canChaseTimer = 0f;
    }

    public override void OnExit()
    {
        Ticker.Instance.OnTick -= Ticker_OnTick;
    }

    private void Ticker_OnTick()
    {
        if (follower.Target == null) return;

        if(follower.Distance(follower.Target) > MaxCircleRadius)
        {
            follower.ChangeState(follower.FollowerChaseState);
            return;
        }

        if (follower.Distance(follower.Target) < follower.FollowerAttackState.AttackRange)
        {
            Vector3 attackDir = follower.Target.transform.position - follower.transform.position;
            follower.FollowerAttackState.SetAttackDirection(attackDir);
            follower.ChangeState(follower.FollowerReadyAttackState);
            return;
        }

        TryToChasePlayer();
    }

    public override void OnUpdate()
    {
        follower.ApplyGravity();

        if (follower.Target == null)
        {
            follower.ChangeState(follower.FollowerWanderState);
            return;
        }

        if (follower.IsBlockedFromEntity(follower.Target))
        {
            follower.ChangeState(follower.FollowerChaseState);
            return;
        }

        canChaseTimer += follower.LocalDeltaTime;

        changeDirTimer += follower.LocalDeltaTime;

        if(changeDirTimer > ChangeDirectionInterval)
        {
            changeDirTimer = 0f;
            follower.SetDestination(CalculateCircleDestination());
            cwCircle = Random.Range(0, ChangeDirectionReciprocal) == 0 ? !cwCircle : cwCircle;
        }

        follower.MoveTowardsDestination(false);
        follower.LookAt(follower.Target.transform.position);
    }

    private void TryToChasePlayer()
    {
        if (canChaseTimer < canChaseCooldown) return;

        if (follower.Target.TryGetComponent(out Player player))
        {
            List<Follower> playerNearbyFollowers = player.GetNearbyHostileEntitiesByType<Follower>(CircleRadius + 1f, false);

            playerNearbyFollowers = playerNearbyFollowers.Take(CircleFollowerCountThreshold).ToList();

            if (!playerNearbyFollowers.Contains(follower)) return;

            follower.ChangeState(follower.FollowerChaseState);
        }
    }

    private Vector3 CalculateCircumferenceOffset(Vector3 center, Vector3 outside, float radius, float angleOffset)
    {
        Vector3 dirToCenter = outside - center;
        float angle = Mathf.Atan2(dirToCenter.z, dirToCenter.x) + angleOffset * Mathf.Deg2Rad;

        return new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle)) + center;
    }

    private Vector3 CalculateCircleDestination()
    {
        int dirMultiplier = cwCircle ? -1 : 1;

        return CalculateCircumferenceOffset(follower.Target.transform.position, follower.transform.position, CircleRadius, dirMultiplier * 10f);
    }
}
