using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowerChaseState : EnemyChaseState
{
    private Follower follower;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        follower = entity as Follower;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(follower.Target == null)
        {
            follower.ChangeState(follower.FollowerWanderState);
            return;
        }

        if(follower.Distance(follower.Target) < follower.FollowerAttackState.AttackRange)
        {
            Vector3 attackDir = follower.Target.transform.position - follower.transform.position;
            follower.FollowerAttackState.SetAttackDirection(attackDir);
            follower.ChangeState(follower.FollowerAttackState);
            return;
        }

        if (!follower.IsCurrentPathValid())
        {
            follower.ChangeState(follower.FollowerWanderState);
            return;
        }

        if(follower.Distance(follower.Target) < follower.FollowerCircleState.CircleRadius)
        {
            CheckCanCircle();
        }
    }

    private void CheckCanCircle()
    {
        if (follower.Target.TryGetComponent(out Player player))
        {
            List<Follower> playerNearbyFollowers = player.GetNearbyHostileEntitiesByType<Follower>(follower.FollowerCircleState.CircleRadius + 1f, false);

            playerNearbyFollowers = playerNearbyFollowers.Take(follower.FollowerCircleState.CircleFollowerCountThreshold).ToList();

            if (playerNearbyFollowers.Contains(follower)) return;

            follower.ChangeState(follower.FollowerCircleState);
        }
    }
}
