using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerChaseState : EnemyChaseState
{
    private Charger charger;
    
    public ChargerChaseState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    } 

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        base.Update();

        if (charger.Target == null)
        {
            charger.ChangeState(charger.EnemyIdleState);
            return;
        }

        // within range attack/switch to attack state
        // for charger have another if statement to check if too close for
        // long range attack

        if (charger.Distance(charger.Target) > charger.MinFarAttackRange && charger.Distance(charger.Target) < charger.MaxFarAttackRange)
        {
            Debug.Log("moving to Far Attack State");
            Vector3 attackDir = charger.Target.transform.position - charger.transform.position;
            
            charger.ChangeState(charger.ChargerFarAttackState);
        }
        else if (charger.Distance(charger.Target) <= charger.AttackRange)
        {
            Debug.Log("less than min attack range");
            charger.ChangeState(charger.ChargerCloseAttackState);
        }

    }


    public override void FixedUpdate()
    {

    }

}
