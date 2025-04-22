[System.Serializable]
public class EnemyIdleState : EnemyBaseState
{
    public override void OnEnter()
    {
        enemy.PlayDefaultAnimation();

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
