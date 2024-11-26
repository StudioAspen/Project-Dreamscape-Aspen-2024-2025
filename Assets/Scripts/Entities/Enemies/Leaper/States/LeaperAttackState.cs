using DG.Tweening;
using UnityEngine;

public class LeaperAttackState : EnemyBaseState
{
    private Leaper leaper;

    private Vector3 destination;

    private float timer;
   
    public float HitBoxRadius;

    private GameObject HitBoxLocation;
    
    private Tween leapTween;
   

    public LeaperAttackState(Leaper enemy) : base(enemy)
    {
        leaper = enemy;
    }

    // public void SetLungeDestination(Vector3 dest)
    // {
    //     destination = dest;
    // }

    public override void OnEnter()
    {
        // leaper.DefaultTransitionToAnimation("FlatMovement");

        // leaper.SetSpeedModifier(2f);
        
        // Debug.Log(leaper.Target);

        // SetLungeDestination(leaper.Target.transform.position);

        // timer = 0;

        // leaper.LookAt(destination);

        // leapTween = leaper.TweenLeap(destination, leaper.LeapAttackDuration, leaper.LeapAttackHeight);
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        // Debug.Log("IN ATTACK STATE");
    }

    public override void FixedUpdate()
    {
        
            if(leaper.Target == null)
            {
                leaper.ChangeState(leaper.LeaperPatrolState);
                return;
            }
            // Vector3 dir = (destination - leaper.transform.position);
            // leapTween;
            leaper.TweenLeap(destination, leaper.LeapAttackDuration, leaper.LeapAttackHeight);
        // leaper.Move(dir);
            
            // leaper.CheckForHits();
            // timer += Time.deltaTime;
            // if (timer > leaper.LungeDuration)
            // {
            //     leaper.ChangeState(leaper.EntityEmptyState);
            //     leaper.ChangeState(leaper.LeaperPatrolState);
            //     return;
            // }    
        
            // leaper.ChangeState(leaper.LeaperPatrolState);
        
        
        
    }
}
