using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChargerJabRecoverState : ChargerBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float JabRecoverDuration { get; private set; } = 2f;

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

        if (timer > JabRecoverDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }

        charger.TransitionToAnimation("RightJab");
    }
}