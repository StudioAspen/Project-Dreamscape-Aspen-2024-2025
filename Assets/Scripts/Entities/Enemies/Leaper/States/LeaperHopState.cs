using System.Collections;
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

        leaper.StartCoroutine(Jump());
        CoinToss();
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

    public IEnumerator Jump()
    {
        Vector3 startPosition = leaper.transform.position;

        for (int i = 0; i < leaper.hopCount; i++)
        {
            Vector3 hopDirection = -leaper.transform.forward * leaper.hopDistance;
            Vector3 targetPositionHop = startPosition + hopDirection;
            float hopPastedTime = 0f;

            while (hopPastedTime < leaper.hopDuration)
            {
                hopPastedTime += Time.deltaTime;
                float t = Mathf.Clamp01(hopPastedTime / leaper.hopDuration);
                Vector3 currentPosition = Vector3.Lerp(startPosition, targetPositionHop, t);
                currentPosition.y += leaper.hopHeight * Mathf.Sin(t * Mathf.PI);
                leaper.transform.position = currentPosition;

                yield return null;
            }

            leaper.transform.position = targetPositionHop;
            startPosition = targetPositionHop;
        }

    }

    public void CoinToss()
    {
        if (leaper.coinToss == 1)
        {
            //leaper.ChangeState(leaper.LeaperAttackState);
        }
        else
        {
            //leaper.ChangeState(leaper.LeaperIdleState);
        }
    }
}

