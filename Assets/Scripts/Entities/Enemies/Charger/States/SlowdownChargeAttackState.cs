using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowdownChargeAttackState : EnemyBaseState
{
    private Charger charger;

    private float currentSpeed;

    private bool hasStopped = false;

    private float slowAcceleration;
    private float turnRateSlowing = 10f; //can be moved into charger.cs


    public SlowdownChargeAttackState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        currentSpeed = charger.ChargeSpeed;
        hasStopped = false;

        //Same as Charging.
        slowAcceleration = charger.ChargeSpeed / charger.SlowDownDuration;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
        //null check.
        if (charger.Target == null)
        {
            Debug.Log("Target is Null: No Where to charge(slow)...");
            charger.ChangeState(charger.SlowdownChargeAttackState); //loop for testing. (should be Idle)

            return;
        }

        charger.CheckForHits();

        //direction and rotation.
        Vector3 dir = (charger.Target.transform.position - charger.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(dir);

        charger.transform.rotation = Quaternion.RotateTowards(charger.transform.rotation, targetRotation, turnRateSlowing * Time.fixedDeltaTime);

        //movement.
        charger.transform.position += charger.transform.forward * currentSpeed * Time.fixedDeltaTime;


        if(!hasStopped)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, slowAcceleration * Time.fixedDeltaTime);

            if (currentSpeed <= 0f)
            {
                hasStopped = true;
            }
        }
        else
        {
            charger.ChangeState(charger.prepareChargeAttackState); //loop back to begining for tesing. 
        }

    }
}
