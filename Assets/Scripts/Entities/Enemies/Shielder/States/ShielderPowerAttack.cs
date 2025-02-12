using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShielderPowerAttack : ShielderBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float AttackRange { get; private set; } = 6f;
    [field: SerializeField] public float AttackPercentDamage { get; private set; } = 150f;

    public Weapon Weapon { get; protected set; }
    private Vector3 attackDirection;

    private Entity rememberedTarget;

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }


    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        Weapon = entity.GetComponentInChildren<Weapon>();
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

        Weapon.OnWeaponStartSwing?.Invoke(shielder);
        Weapon.ClearEnemiesHitList();

        Weapon.SetPercentDamage(AttackPercentDamage);

        shielder.IsAttackAnimationPlaying = true;
        shielder.UseRootMotion = true;
    }

    public override void OnExit()
    {
        Weapon.OnWeaponEndSwing?.Invoke(shielder);
        shielder.IsAttackAnimationPlaying = false;
        shielder.UseRootMotion = false;
        shielder.EndHit();
    }

    public override void OnUpdate()
    {
        shielder.ApplyGravity();

        shielder.LookAt(shielder.transform.position + attackDirection);

        //After the Power Attack Animation is done changes to specified state.
        if (!shielder.IsAttackAnimationPlaying)
        {
            shielder.ChangeState(shielder.ShielderIdleState);
            return;
        }
    }

}
