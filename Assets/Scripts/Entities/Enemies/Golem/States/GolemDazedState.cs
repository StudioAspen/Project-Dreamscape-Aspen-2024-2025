using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GolemDazedState : GolemBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float DazedDuration { get; private set; } = 5f;

    private float timer;

    public override void OnEnter()
    {
        golem.PlayOneShotAnimation(AnimationClip);
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