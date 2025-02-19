using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyChaseState : EnemyBaseState
{
    public override void OnEnter()
    {
        enemy.PlayDefaultAnimation();

        enemy.SetSpeedModifier(1f);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        enemy.ApplyGravity();

        if (enemy.Target == null)
        {
            enemy.ChangeState(enemy.DefaultState);
            return;
        }

        enemy.SetDestination(enemy.Target.transform.position);
        enemy.MoveTowardsDestination();
    }
}
