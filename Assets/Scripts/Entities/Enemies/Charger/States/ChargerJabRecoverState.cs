using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChargerJabRecoverState : ChargerBaseState
{
    private float timer;

    public override void OnEnter()
    {
        charger.TransitionToAnimation("RightJab");

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        charger.ApplyGravity();

        timer += charger.LocalDeltaTime;

        if (timer > charger.JabRecoverDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }

        charger.TransitionToAnimation("RightJab");
    }
}