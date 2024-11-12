using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerTargetDetectedState : EnemyBaseState
{
    private Charger charger;

    private Entity rememberedTarget;

    private float timer;

    public ChargerTargetDetectedState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        charger.DefaultTransitionToAnimation("TargetDetected");

        charger.SetSpeedModifier(0f);

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        timer += Time.deltaTime;

        charger.LookAt(rememberedTarget.transform.position);
        
        if(timer > charger.TargetDetectedDuration)
        {
            float distanceToTarget = charger.Distance(rememberedTarget.transform.position);

            if(distanceToTarget < charger.NearbyAttackRadiusThreshold)
            {
                // change to nearby attack state

                // temporary for now
                charger.ChargerChargeState.AssignCurrentRememberedTarget(rememberedTarget);
                charger.ChangeState(charger.ChargerChargeState);
            }
            else
            {
                charger.ChargerChargeState.AssignCurrentRememberedTarget(rememberedTarget);
                charger.ChangeState(charger.ChargerChargeState);
            }

            return;
        }
    }

    public override void FixedUpdate()
    {

    }
}
