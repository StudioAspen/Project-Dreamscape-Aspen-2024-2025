using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChargerIdleState : EnemyIdleState
{
    private Coroutine wanderCoroutine;
    private Charger charger;

    public ChargerIdleState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        charger.SetSpeedModifier(1f);
        wanderCoroutine = charger.StartCoroutine(WanderCoroutine());
        //Debug.Log("Started charger wander coroutine");
    }

    public override void OnExit()
    {
        if (wanderCoroutine != null)
        {
            charger.StopCoroutine(wanderCoroutine);
            wanderCoroutine = null;
            //Debug.Log("Stopped charger wander coroutine");
        }
    }

    public override void Update()
    {
        if (charger.Target != null)
        {
            //charger.ChangeState(charger.ChargerPlayerDetectedState);
        }
    }


    public override void FixedUpdate() { }


    private IEnumerator WanderCoroutine()
    {
        while (charger.CurrentState == this)
        {
            yield return new WaitForSeconds(Random.Range(charger.WanderWaitMin, charger.WanderWaitMax));
            GoToRandomWanderPoint();
        }
    }

    private void GoToRandomWanderPoint()
    {
        Vector3 randomPoint = (Random.insideUnitSphere * charger.WanderRadius) + charger.transform.position;
        charger.SetDestination(randomPoint, true);
    }

}