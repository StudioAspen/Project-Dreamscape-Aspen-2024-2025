using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prepareChargeAttackState : EnemyBaseState
{
    private Charger charger;

    private bool completeLookAt = false;

    public prepareChargeAttackState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        completeLookAt = false;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (charger.Target == null)
        {
            Debug.Log("Target has not been assigned: Returning to Default...");
            charger.ChangeState(charger.prepareChargeAttackState); //loop for testing. (Should be Idle)
        }
    }

    public override void FixedUpdate()
    {
        if (charger.Target != null)
        {
            //looks towards target direction.
            Debug.Log("The Target is: " + enemy.Target);
            charger.LookAt(charger.Target.transform.position);


            //formula to check the angle between charger forward and the target.
            float approxAngle = Vector3.Angle(charger.transform.forward, (charger.Target.transform.position - charger.transform.position).normalized);

            if (approxAngle < 5f)
            {
                completeLookAt = true;
            }

            if (completeLookAt)
            {
                charger.ChangeState(charger.ChargeAttackState);
            }
        }
    }
}
