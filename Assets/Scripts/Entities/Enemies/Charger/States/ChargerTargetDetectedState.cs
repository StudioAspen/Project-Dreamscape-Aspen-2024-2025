using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChargerTargetDetectedState : ChargerBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; protected set; }
    [field: SerializeField] public float TargetDetectedDuration { get; private set; } = 2f;
    [field: SerializeField] public float NearbyAttackRadiusThreshold { get; private set; } = 6f;

    private Entity rememberedTarget;

    private float timer;

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        charger.PlayOneShotAnimation(AnimationClip, TargetDetectedDuration);

        charger.SetSpeedModifier(0f);

        charger.ChargerJabbingAttackState.ResetJabCount();

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

        timer += charger.LocalDeltaTime;
        
        if(timer > TargetDetectedDuration)
        {
            float distanceToTarget = charger.Distance(rememberedTarget.transform.position);

            if(distanceToTarget < NearbyAttackRadiusThreshold)
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
