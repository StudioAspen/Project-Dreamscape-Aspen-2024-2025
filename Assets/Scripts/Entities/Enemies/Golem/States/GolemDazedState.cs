using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemDazedState : GolemBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float DazedDuration { get; private set; } = 5f;

    private float timer;

    public override void OnEnter()
    {
        golem.TransitionToAnimation("Daze");
        golem.SetSpeedModifier(0f);
        timer = 0f;
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        golem.ApplyGravity();
        timer += golem.LocalDeltaTime;

        if(timer > DazedDuration)
        {
            golem.ChangeState(golem.GolemWanderState);
            return;
        }
    }
}