using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class ShielderQuickAttackState : ShielderBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float AttackDuration { get; private set; } = 2f;
    [field: SerializeField] public float AttackRange { get; private set; } = 1.5f;
    [field: SerializeField] public float AttackDamageMultiplier { get; private set; } = 1f;

    private Vector3 attackDirection;
    private float timer;

    public void SetAttackDirection(Vector3 direction)
    {
        attackDirection = direction;
    }

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

        if (shielder.Target == null)
        {
            shielder.ChangeState(shielder.ShielderWanderState);
            return;
        }

        timer += shielder.LocalDeltaTime;
        if(timer > AttackDuration)
        {
            // Check if in range for power attack
            if(shielder.Distance(shielder.Target) < shielder.ShielderPowerAttackState.AttackRange)
            {
                shielder.ChangeState(shielder.ShielderPowerAttackState);
            }
            else
            {
                shielder.ChangeState(shielder.ShielderDefensiveState);
            }
            return;
        }

        shielder.LookAt(shielder.transform.position + attackDirection);
    }
}
