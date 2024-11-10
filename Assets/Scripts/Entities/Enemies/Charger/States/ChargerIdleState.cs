using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargerIdleState : EnemyIdleState
{
    private Coroutine wanderTimerCoroutine;
    private Charger charger;

    public ChargerIdleState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        enemy.DefaultTransitionToAnimation("Idle");
        charger.SetSpeedModifier(0f);
        wanderTimerCoroutine = charger.StartCoroutine(WanderTimerCoroutine());


    }

    public override void OnExit()
    {
        if (wanderTimerCoroutine != null)
        {
            charger.StopCoroutine(wanderTimerCoroutine);
            wanderTimerCoroutine = null;
     
        }
    }

    public override void Update()
    {
        // NOTE: should probably remove since WanderTimerCoroutine runs first
        // and changes to WanderState instantly 
        // Update that is not the case lol
        if (charger.Target != null)
        {
            Debug.Log("change to chase");
            charger.ChangeState(charger.ChargerChaseState);
        }
    }


    private IEnumerator WanderTimerCoroutine()
    {
        while (charger.CurrentState == this)
        {
            Debug.Log("change to wander from idle");
            yield return new WaitForSeconds(Random.Range(charger.IdleWanderWaitMin, charger.IdleWanderWaitMax));
            charger.ChangeState(charger.ChargerWanderState);
        }
        yield return null;
    }

    public override void FixedUpdate() { }

}
