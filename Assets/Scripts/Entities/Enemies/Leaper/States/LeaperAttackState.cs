using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class LeaperAttackState : EnemyBaseState
{
    private Leaper leaper;

    public LeaperAttackState(Leaper enemy) : base(enemy)
    {
        leaper = enemy;
    }

    public override void OnEnter()
    {
        
        leaper.debugTimerDuration = 0;
        
        leaper.DefaultTransitionToAnimation("FlatMovement");

    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {

        // NOTE: changed script to use IEnumerator Jump
        // as previous code did not work on my end 
        // have another timer as another delay is necessary
        leaper.debugTimerDuration += Time.deltaTime;
        // Debug.Log(leaper.debugTimerDuration);
        if (leaper.debugTimerDuration > 3)
        {
            leaper.StartCoroutine(Jump());  
            leaper.CheckForHits();
            leaper.debugTimerDuration = 0;
            
        }
    }

    public override void FixedUpdate()
    {           
        
    }

    public IEnumerator Jump()
    {
        Vector3 startPosition = leaper.transform.position;

        for (int i = 0; i < 1; i++)
        {
            Vector3 hopDirection = leaper.transform.forward * leaper.HopDistance;
            Vector3 targetPositionHop = leaper.Target.transform.position;
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
        leaper.ChangeState(leaper.LeaperPatrolState);
        
    }
}
