using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerDazedState : ChargerBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float DazedDuration { get; private set; } = 5f;

    private float timer;

    public override void OnEnter()
    {
        charger.TransitionToAnimation("Hit");

        charger.SetSpeedModifier(0f);

        timer = 0f;
    }

    public override void OnExit()
    {

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
