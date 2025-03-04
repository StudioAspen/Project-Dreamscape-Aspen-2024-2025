using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ChargerJabRecoverState : ChargerBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float JabRecoverDuration { get; private set; } = 2f;

    private float timer;

    public override void OnEnter()
    {
        charger.PlayOneShotAnimation(AnimationClip, JabRecoverDuration);

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

        if (timer > JabRecoverDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }
    }
}