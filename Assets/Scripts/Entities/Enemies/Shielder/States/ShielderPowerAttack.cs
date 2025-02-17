using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShielderPowerAttack : ShielderBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float AttackRange { get; private set; } = 5f;
    [field: SerializeField] public float AttackPercentDamage { get; private set; } = 250f;

    [field: SerializeField] public Weapon LongSword { get; protected set; }

    private Vector3 attackDirection;
    private Entity rememberedTarget;

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
        //Start the Power Attack Animation (Functions controlled through Anim Events)
        shielder.TransitionToAnimation("PowerAttack");
        shielder.SetSpeedModifier(0f);

        //NEW FUNCTION USED TO SCALE COLLIDER FOR THE POWER ATTACK TO HAVE MORE IMPACT.
        LongSword.ColliderAdjustment(.4f, 1.663404f);

        LongSword.OnWeaponStartSwing?.Invoke(shielder);
        LongSword.ClearEnemiesHitList();
        LongSword.SetPercentDamage(AttackPercentDamage);

        shielder.IsAttackAnimationPlaying = true;
        shielder.UseRootMotion = true;

    }

    public override void OnExit()
    {
        LongSword.ColliderAdjustment(.25f, 1.663404f);
        shielder.IsAttackAnimationPlaying = false;
        shielder.UseRootMotion = false;

        LongSword.OnWeaponEndSwing?.Invoke(shielder);

        shielder.EndHit();
    }

    public override void OnUpdate()
    {
        shielder.ApplyGravity();

        shielder.LookAt(shielder.transform.position + attackDirection);

        if (!shielder.IsAttackAnimationPlaying)
        {
            if ((shielder.Distance(shielder.Target) < shielder.ShielderPowerAttackState.AttackRange) && (shielder.Target.CurrentState == shielder.Target.EntityLaunchState))
            {
                shielder.ShielderPowerAttackState.SetAttackDirection(attackDirection);
                shielder.ChangeState(shielder.ShielderPowerAttackState);

                return;
            }

            shielder.ChangeState(shielder.ShielderDefensiveState);
            return;
        }
    }

}
