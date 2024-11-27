using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
        
        leaper.debugTimerDuration = 0;
        leaper.SetSpeedModifier(2);
        
        
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
        
        timer += Time.deltaTime;
            // previous code works but was very buggy and would only work sometimes
            // might just be on my end so ill keep it here 
        
        if (timer < leaper.LeapAttackDuration)
        {
            if(leaper.Target == null)
            {
                leaper.ChangeState(leaper.LeaperPatrolState);
                return;
            }
            // leaper.TweenLeap(destination, leaper.LeapAttackDuration, leaper.LeapAttackHeight);
            leaper.StartCoroutine(Jump());
            // leaper.CheckForHits();
        }   
        else
        {
            leaper.ChangeState(leaper.LeaperPatrolState);
        }
    }

    public override void FixedUpdate()
    {   


        // leaper.ChangeState(leaper.LeaperPatrolState);


            // Vector3 dir = (destination - leaper.transform.position);
            // leapTween;
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

    public IEnumerator Jump()
    {
        Vector3 startPosition = leaper.transform.position;

        Vector3 hopDirection = -leaper.transform.forward * leaper.HopDistance;
        Vector3 targetPositionHop = startPosition + hopDirection;
        float hopPastedTime = 0f;

        while (hopPastedTime < leaper.HopDuration)
        {
            hopPastedTime += Time.deltaTime;
            float t = Mathf.Clamp01(hopPastedTime / leaper.HopDuration);
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPositionHop, t);
            currentPosition.y += leaper.HopHeight * Mathf.Sin(t * Mathf.PI);
            leaper.transform.position = currentPosition;

            yield return null;
        }

        leaper.transform.position = targetPositionHop;
        startPosition = targetPositionHop;
        
    }
}
