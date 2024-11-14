using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaperHopState : EnemyBaseState
{
    private Leaper leaper;

    public LeaperHopState(Leaper enemy) : base(enemy)
    {
        leaper = enemy;
    }

    public override void OnEnter()
    {
        leaper.DefaultTransitionToAnimation("Hop");

        Jump();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
        
    }

    public void Jump()
    {
        Transform rootTransform = leaper.enemyTransform.parent;
        Vector3 startPosition = rootTransform.position;

        for (int i = 0; i < leaper.hopCount; i++)
        {
            Vector3 hopDirection = -rootTransform.forward * leaper.hopDistance;
            Vector3 targetPositionHop = startPosition + hopDirection;
            float hopPastedTime = 0f;

            while (hopPastedTime < leaper.hopDuration)
            {
                hopPastedTime += Time.deltaTime;
                float t = Mathf.Clamp01(hopPastedTime / leaper.hopDuration);
                Vector3 currentPosition = Vector3.Lerp(startPosition, targetPositionHop, t);
                currentPosition.y += leaper.hopHeight * Mathf.Sin(t * Mathf.PI);
                rootTransform.position = currentPosition;

                yield return null;
            }

            rootTransform.position = targetPositionHop;
            startPosition = targetPositionHop;
        }

        //transition state 50/50
        //leaper.ChangeState(leaper.LeaperAttackState);
        return;

    }
}

