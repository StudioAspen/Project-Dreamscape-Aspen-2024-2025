using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseStateSO : EnemyBaseStateSO
{
    public override void OnEnter()
    {
        enemy.TransitionToAnimation("FlatMovement");

        enemy.SetSpeedModifier(1f);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        enemy.ApplyGravity();

        if (enemy.Target == null)
        {
            enemy.ChangeState(enemy.EnemyIdleState);
            return;
        }

        enemy.SetDestination(enemy.Target.transform.position);
        enemy.MoveTowardsDestination();
    }

    public override void FixedUpdate()
    {

    }
}
