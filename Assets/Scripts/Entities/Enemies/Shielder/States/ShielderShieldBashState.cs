using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   
public class ShielderShieldBashState : ShielderBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float Duration { get; private set; } = 1f;
    [field: SerializeField] public float AttackDamageMultiplier { get; private set; } = 0.5f;
    [field: SerializeField] public float ShieldBashStunTime { get; private set; } = 1f;

    private float timer;
    private Vector3 attackDirection;

    public void SetAttackDirection(Vector3 direction)
    {
        attackDirection = direction;
    }

    public override void OnEnter()
    {
        shielder.PlayOneShotAnimation(AnimationClip, Duration);
        shielder.SetSpeedModifier(0f);

        shielder.Shield.OnWeaponStartSwing?.Invoke(shielder, null);
        shielder.Shield.ClearObjectHitList();
        shielder.Shield.SetDamageMultiplier(AttackDamageMultiplier);

        shielder.UseRootMotion = true;

        timer = 0f;

        shielder.Shield.OnWeaponHit += Shielder_Shield_OnWeaponHit;
    }

    public override void OnExit()
    {
        shielder.Shield.OnWeaponHit -= Shielder_Shield_OnWeaponHit;

        shielder.UseRootMotion = false;
        shielder.Shield.OnWeaponEndSwing?.Invoke(shielder, null);
        shielder.Shield.DisableTriggers();
    }

    public override void OnUpdate()
    {
        shielder.ApplyGravity();

        timer += shielder.LocalDeltaTime;
        if(timer > Duration)
        {
            shielder.ChangeState(shielder.ShielderDefensiveState);
            return;
        }

        shielder.LookAt(shielder.transform.position + attackDirection);
    }

    private void Shielder_Shield_OnWeaponHit(Entity attacker, Entity victim, Vector3 hitPoint, int damage)
    {
        victim.EntityStunnedState.StunEntity(shielder, ShieldBashStunTime);
    }
}


