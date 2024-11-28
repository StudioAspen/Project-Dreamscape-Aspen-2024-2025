using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowerChaseState : EnemyChaseState
{
    private Follower follower;

    public FollowerChaseState(Follower enemy) : base(enemy)
    {
        follower = enemy;
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
            follower.ChangeState(follower.EnemyIdleState);
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

    private void CheckCanCircle()
    {
        if (follower.Target.TryGetComponent(out Player player))
        {
            List<Follower> playerNearbyFollowers = player.GetNearbyHostileEntitiesByType<Follower>(follower.CircleRadius + 1f);

            foreach (Follower f in new List<Follower>(playerNearbyFollowers)) // filter so that we only look for followers that are alive
            {
                if (f.CurrentState == f.EntityDeathState) playerNearbyFollowers.Remove(f);
            }

            playerNearbyFollowers = playerNearbyFollowers.Take(follower.CircleFollowerCountThreshold).ToList();

            if (playerNearbyFollowers.Contains(follower)) return;

            follower.ChangeState(follower.FollowerCircleState);
        }
    }

    public override void FixedUpdate()
    {

    }
}
