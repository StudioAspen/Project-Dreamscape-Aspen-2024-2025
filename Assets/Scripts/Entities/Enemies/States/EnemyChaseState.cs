using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(Enemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

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
