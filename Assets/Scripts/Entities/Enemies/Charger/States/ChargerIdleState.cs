using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            Debug.LogWarning("exit idle");
        }
    }

    public override void Update()
    {
        if (charger.Target != null)
        {
            charger.ChangeState(charger.ChargerPlayerDetectedState);
        }
    }


    private IEnumerator WanderTimerCoroutine()
    {
        while (charger.CurrentState == this)
        {
            yield return new WaitForSeconds(Random.Range(charger.IdleWanderWaitMin, charger.IdleWanderWaitMax));
            charger.ChangeState(charger.ChargerWanderState);
        }
        yield return null;
    }

    public override void FixedUpdate() { }


    

}