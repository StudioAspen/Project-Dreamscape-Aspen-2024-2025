using UnityEngine;

public class LeaperAttackState : EnemyBaseState
{
    private Leaper leaper;

    private Vector3 destination;

    private float timer;

    public LeaperAttackState(Leaper enemy) : base(enemy)
    {
        leaper = enemy;
    }

    public void SetLungeDestination(Vector3 dest)
    {
        destination = dest;
    }

    public override void OnEnter()
    {
        leaper.DefaultTransitionToAnimation("FlatMovement");

        leaper.SetSpeedModifier(2f);

        SetLungeDestination(leaper.Target.transform.position);

        timer = 0;
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        CheckForHits();
        timer += Time.deltaTime;
        if (timer > leaper.LungeDuration)
        {
            leaper.ChangeState(leaper.EntityEmptyState);
            leaper.ChangeState(leaper.LeaperAttackState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        Vector3 dir = (destination - leaper.transform.position).normalized;

        leaper.Move(dir);
    }

    private void CheckForHits()
    {

    }
}
