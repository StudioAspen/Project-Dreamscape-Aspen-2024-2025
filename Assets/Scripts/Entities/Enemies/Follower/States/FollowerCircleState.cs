using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowerCircleState : EnemyBaseState
{
    private Follower follower;

    private bool cwCircle;

    private float changeDirTimer;

    private float canChaseTimer;
    private float canChaseCooldown = 3f;

    public FollowerCircleState(Follower enemy) : base(enemy)
    {
        follower = enemy;
    }

    public override void OnEnter()
    {
        follower.TransitionToAnimation("FlatMovement");

        follower.SetSpeedModifier(0.5f);

        Ticker.Instance.OnTick.AddListener(Ticker_OnTick);

        changeDirTimer = 0f;

        canChaseTimer = 0f;
    }

    public override void OnExit()
    {
        Ticker.Instance.OnTick.RemoveListener(Ticker_OnTick);
    }

    private void Ticker_OnTick()
    {
        if (follower.Target == null) return;

        if(follower.Distance(follower.Target) > follower.MaxCircleRadius)
        {
            follower.ChangeState(follower.EnemyChaseState);
            return;
        }

        if (follower.Distance(follower.Target) < follower.AttackRange)
        {
            Vector3 attackDir = follower.Target.transform.position - follower.transform.position;
            follower.FollowerAttackState.SetAttackDirection(attackDir);
            follower.ChangeState(follower.FollowerReadyAttackState);
            return;
        }

        TryToChasePlayer();
    }

    public override void Update()
    {
        if(follower.Target == null)
        {
            follower.ChangeState(follower.EnemyIdleState);
            return;
        }

        if (follower.IsBlockedFromEntity(follower.Target))
        {
            follower.ChangeState(follower.EnemyChaseState);
            return;
        }

        canChaseTimer += Time.deltaTime;

        changeDirTimer += Time.deltaTime;

        if(changeDirTimer > follower.ChangeDirectionInterval)
        {
            changeDirTimer = 0f;
            follower.SetDestination(CalculateCircleDestination(), false);
            cwCircle = Random.Range(0, follower.ChangeDirectionReciprocal) == 0 ? !cwCircle : cwCircle;
        }

        follower.LookAt(follower.Target.transform.position);
    }

    private void TryToChasePlayer()
    {
        if (canChaseTimer < canChaseCooldown) return;

        if (follower.Target.TryGetComponent(out Player player))
        {
            List<Follower> playerNearbyFollowers = player.GetNearbyHostileEntitiesByType<Follower>(follower.CircleRadius + 1f);

            foreach (Follower f in new List<Follower>(playerNearbyFollowers)) // filter so that we only look for followers that are alive
            {
                if (f.CurrentState == f.EntityDeathState) playerNearbyFollowers.Remove(f);
            }

            playerNearbyFollowers = playerNearbyFollowers.Take(follower.CircleFollowerCountThreshold).ToList();

            if (!playerNearbyFollowers.Contains(follower)) return;

            follower.ChangeState(follower.EnemyChaseState);
        }
    }

    public override void FixedUpdate()
    {

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

        return CalculateCircumferenceOffset(follower.Target.transform.position, follower.transform.position, follower.CircleRadius, dirMultiplier * 10f);
    }
}
