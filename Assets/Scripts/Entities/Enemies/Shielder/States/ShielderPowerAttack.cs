using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShielderPowerAttack : ShielderBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float AttackDuration { get; private set; } = 2.5f;
    [field: SerializeField] public float AttackRange { get; private set; } = 2f;
    [field: SerializeField] public float AttackDamageMultiplier { get; private set; } = 2.5f;

    private float timer;

    public override void OnEnter()
    {
        shielder.PlayOneShotAnimation(AnimationClip, AttackDuration);
        shielder.SetSpeedModifier(0f);

        shielder.LongSword.OnWeaponStartSwing?.Invoke(shielder, null);
        shielder.LongSword.ClearObjectHitList();
        shielder.LongSword.SetDamageMultiplier(AttackDamageMultiplier);

        shielder.UseRootMotion = true;

        timer = 0f;
    }

    public override void OnExit()
    {
        shielder.UseRootMotion = false;
        shielder.LongSword.OnWeaponEndSwing?.Invoke(shielder, null);
        shielder.LongSword.DisableTriggers();
    }

    public override void OnUpdate()
    {
        shielder.ApplyGravity();

        if(shielder.Target == null)
        {
            shielder.ChangeState(shielder.ShielderWanderState);
            return;
        }

        timer += shielder.LocalDeltaTime;
        if(timer > AttackDuration)
        {
            shielder.ChangeState(shielder.ShielderDefensiveState);
            return;
        }

        shielder.LookAt(shielder.Target.transform.position);
    }
}
