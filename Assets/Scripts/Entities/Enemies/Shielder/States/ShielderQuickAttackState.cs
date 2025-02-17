using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ShielderQuickAttackState : ShielderBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float AttackRange { get; private set; } = 2f;
    [field: SerializeField] public float AttackPercentDamage { get; private set; } = 100f;

    [field: SerializeField] public Weapon LongSword { get; protected set; }

    private Entity rememberedTarget;
    private Vector3 attackDirection;

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    private protected override void Init(Entity entity)
    {
        base.Init(entity);
    }

    public void SetAttackDirection(Vector3 direction)
    {
        attackDirection = direction;
    }

    public override void OnEnter()
    {
        Debug.Log("Quick Attack Entered!!");

        //Start the Quick Attack Animation (Functions controlled through Anim Events)
        shielder.TransitionToAnimation("QuickAttack");
        shielder.SetSpeedModifier(0f);

        LongSword.OnWeaponStartSwing?.Invoke(shielder);
        LongSword.ClearEnemiesHitList();
        LongSword.SetPercentDamage(AttackPercentDamage);

        shielder.IsAttackAnimationPlaying = true;
        shielder.UseRootMotion = true;
    }

    public override void OnExit()
    {
        shielder.IsAttackAnimationPlaying = false;
        shielder.UseRootMotion = false;

        LongSword.OnWeaponEndSwing?.Invoke(shielder);

        shielder.EndHit();
    }

    public override void OnUpdate()
    {
        shielder.ApplyGravity();

        shielder.LookAt(shielder.transform.position + attackDirection);

        //After the attack animation is done, check if the target is still in range of the Power Attack, if so goes to Power Attack state, else goes to Idle state.
        if (!shielder.IsAttackAnimationPlaying && shielder.Distance(shielder.Target) < shielder.ShielderPowerAttackState.AttackRange)
        {
            Vector3 attackDir = shielder.Target.transform.position - shielder.transform.position;
            shielder.ShielderPowerAttackState.SetAttackDirection(attackDir);
            shielder.ChangeState(shielder.ShielderPowerAttackState);
            return;
        }
        else if (!shielder.IsAttackAnimationPlaying && shielder.Distance(shielder.Target) > shielder.ShielderPowerAttackState.AttackRange)
        {
            shielder.ChangeState(shielder.ShielderDefensiveState);
            return;
        }
    }
}
