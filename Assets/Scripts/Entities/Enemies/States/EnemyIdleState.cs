public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(Enemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void OnEnter()
    {
        enemy.TransitionToAnimation("FlatMovement");

        enemy.SetSpeedModifier(0f);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        enemy.ApplyGravity();

        if (enemy.Target != null)
        {
            enemy.ChangeState(enemy.EnemyChaseState);
        }
    }

    public override void FixedUpdate()
    {
        
    }
}
