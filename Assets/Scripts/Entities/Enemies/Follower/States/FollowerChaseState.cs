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

        if (follower.Target.TryGetComponent(out Player player))
        {
            if (player.NearbyEntities.Count > 0)
            {
                bool qualifiedToChase = false;

                for (int i = 0; i < Mathf.Min(follower.CircleEntityCountThreshold, player.NearbyEntities.Count); i++)
                {
                    if (player.NearbyEntities[i].gameObject == follower.gameObject)
                    {
                        qualifiedToChase = true;
                    }
                }

                if (!qualifiedToChase)
                {
                    if (follower.Distance(follower.Target) < follower.MaxCircleRadius)
                    {
                        follower.ChangeState(follower.FollowerCircleState);
                    }
                }
            }
        }
    }

    public override void FixedUpdate()
    {

    }
}
