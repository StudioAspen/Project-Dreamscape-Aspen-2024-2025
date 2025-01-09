using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerTargetDetectedState : ChargerBaseState
{
    private Entity rememberedTarget;

    private float timer;

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        charger.TransitionToAnimation("TargetDetected");

        charger.SetSpeedModifier(0f);

        charger.ResetJabCount();

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        charger.ApplyGravity();

        if (rememberedTarget == null)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }

        charger.LookAt(rememberedTarget.transform.position);

        timer += charger.LocalDeltaTime;
        
        if(timer > charger.TargetDetectedDuration)
        {
            float distanceToTarget = charger.Distance(rememberedTarget.transform.position);

            if(distanceToTarget < charger.NearbyAttackRadiusThreshold)
            {
                charger.ChargerJabbingAttackState.AssignCurrentRememberedTarget(rememberedTarget);
                charger.ChangeState(charger.ChargerJabbingAttackState);
            }
            else
            {
                charger.ChargerChargeState.AssignCurrentRememberedTarget(rememberedTarget);
                charger.ChangeState(charger.ChargerChargeState);
            }

            return;
        }
    }
}
