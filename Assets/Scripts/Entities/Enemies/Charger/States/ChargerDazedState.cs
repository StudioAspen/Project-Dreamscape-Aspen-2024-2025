using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerDazedState : ChargerBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float DazedDuration { get; private set; } = 5f;

    private float timer;

    public override void OnEnter()
    {
        charger.PlayOneShotAnimation(AnimationClip);

        charger.SetSpeedModifier(0f);

        timer = 0f;
    }

    public override void OnExit()
    {
        charger.PlayDefaultAnimation();
    }

    public override void OnUpdate()
    {
        charger.ApplyGravity();

        timer += charger.LocalDeltaTime;

        if(timer > DazedDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }
    }
}
