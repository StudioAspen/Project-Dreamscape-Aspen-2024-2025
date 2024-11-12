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

    public override void OnEnter()
    {
        charger.SetSpeedModifier(0f);
        
        rememberedTarget = charger.Target;

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
