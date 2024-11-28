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
        charger.TransitionToAnimation("TargetDetected");

        charger.SetSpeedModifier(0f);

        charger.ResetJabCount();

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (rememberedTarget == null)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }

        timer += charger.LocalDeltaTime;

        charger.LookAt(rememberedTarget.transform.position);
        
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

    public override void FixedUpdate()
    {

    }
}
