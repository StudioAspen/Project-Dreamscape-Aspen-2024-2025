using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttackState : EnemyBaseState
{
    private Charger charger;

    private float currentSpeed = 0f;

    private float holdMaxSpeedTime = 1f; //can be moved into charger.cs
    private float holdMaxSpeedCurrentTime = 0f;

    private bool atMaxSpeed = false;

    private float acceleration;

    private float turnRate = 30f; //can be moved into charger.cs


    //IF Player was hit, go into WINDDOWN STATE

    public ChargeAttackState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        enemy.DefaultTransitionToAnimation("FlatMovement");

        currentSpeed = 0;
        holdMaxSpeedCurrentTime = 0;
        atMaxSpeed = false;

        //acceleration = (final - initial) / time.
        //NOTE: initial = 0
        acceleration = charger.ChargeSpeed / charger.ChargeDuration;
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
            Debug.Log("Target is Null: No Where to charge...");
            charger.ChangeState(charger.ChargeAttackState); //loop for testing. (should be Idle)

            return;
        }

        charger.CheckForHits();

        //direction and rotation.
        Vector3 dir = (charger.Target.transform.position - charger.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(dir);

        charger.transform.rotation = Quaternion.RotateTowards(charger.transform.rotation, targetRotation, turnRate * Time.fixedDeltaTime);

        //movement.
        charger.transform.position += charger.transform.forward * currentSpeed * Time.fixedDeltaTime;


        if (!atMaxSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, charger.ChargeSpeed, acceleration * Time.fixedDeltaTime);

            if (currentSpeed >= charger.ChargeSpeed)
            {
                atMaxSpeed = true;
            }
        }
        else
        {
            holdMaxSpeedCurrentTime += Time.fixedDeltaTime;

            //after max speed is held for x seconds, goes into slow down state.
            if (holdMaxSpeedCurrentTime >=holdMaxSpeedTime)
            {
                charger.ChangeState(charger.SlowdownChargeAttackState);
            }
        }

    }
}
