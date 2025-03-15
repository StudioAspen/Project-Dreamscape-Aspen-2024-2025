using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class ShielderTargetDetectedState : ShielderBaseState 
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float Duration { get; private set; } = 0.5f;
    private float timer;

    public override void OnEnter()
    {
        shielder.PlayOneShotAnimation(AnimationClip);

        shielder.SetSpeedModifier(0f);

        timer = 0f;
        shielder.transform.DORotateQuaternion(shielder.LookAt(shielder.Target.transform.position), Duration);
    }

    public override void OnExit()
    {
        shielder.transform.DOKill();
    }

    public override void OnUpdate()
    {
        shielder.ApplyGravity();

        if (shielder.Target == null)
        {
            shielder.ChangeState(shielder.ShielderWanderState);
            return;
        }

        timer += shielder.LocalDeltaTime;
        if(timer > Duration)
        {
            shielder.ChangeState(shielder.ShielderDefensiveState);
            return;
        }
    }
}

