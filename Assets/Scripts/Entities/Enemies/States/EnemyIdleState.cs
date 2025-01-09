public class EnemyIdleState : EnemyBaseState
{
    public override void OnEnter()
    {
        enemy.TransitionToAnimation("FlatMovement");

        enemy.SetSpeedModifier(0f);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        enemy.ApplyGravity();

        if (enemy.Target != null)
        {
            enemy.ChangeState(enemy.EnemyChaseState);
        }
    }
}
