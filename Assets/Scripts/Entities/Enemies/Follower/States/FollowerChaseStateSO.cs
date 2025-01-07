using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowerChaseStateSO : EnemyChaseStateSO
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

    public override void Update()
    {
        base.Update();

        if(follower.Target == null)
        {
            follower.ChangeState(follower.FollowerWanderState);
            return;
        }

        if(follower.Distance(follower.Target) < follower.AttackRange)
        {
            Vector3 attackDir = follower.Target.transform.position - follower.transform.position;
            follower.FollowerAttackState.SetAttackDirection(attackDir);
            follower.ChangeState(follower.FollowerAttackState);
            return;
        }

        if(follower.Distance(follower.Target) < follower.CircleRadius)
        {
            CheckCanCircle();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void CheckCanCircle()
    {
        if (follower.Target.TryGetComponent(out Player player))
        {
            List<Follower> playerNearbyFollowers = player.GetNearbyHostileEntitiesByType<Follower>(follower.CircleRadius + 1f, false);

            playerNearbyFollowers = playerNearbyFollowers.Take(follower.CircleFollowerCountThreshold).ToList();

            if (playerNearbyFollowers.Contains(follower)) return;

            follower.ChangeState(follower.FollowerCircleState);
        }
    }
}
