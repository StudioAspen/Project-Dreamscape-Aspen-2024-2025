using UnityEngine;

public class LeaperAttackState : EnemyBaseState
{
    private Leaper leaper;

    private Vector3 destination;

    private float timer;
   
    public float HitBoxRadius;

    private GameObject HitBoxLocation;
    

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
        
        Debug.Log(leaper.Target);

        SetLungeDestination(leaper.Target.transform.position);

        timer = 0;

        leaper.LookAt(destination);
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void FixedUpdate()
    {
        if(leaper.Target == null)
        {
            leaper.ChangeState(leaper.LeaperAttackState);
            return;
        }
        Vector3 dir = (destination - leaper.transform.position);

        leaper.Move(dir);
        
        leaper.CheckForHits();
        timer += Time.deltaTime;
        if (timer > leaper.LungeDuration)
        {
            leaper.ChangeState(leaper.EntityEmptyState);
            leaper.ChangeState(leaper.LeaperAttackState);
            return;
        }
        
    }

}
