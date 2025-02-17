using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShielderShieldBashState : ShielderBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float AttackRange { get; private set; } = 2f;
    [field: SerializeField] public float AttackPercentDamage { get; private set; } = 100f;

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
        shielder.TransitionToAnimation("ShieldBash");
        shielder.SetSpeedModifier(0f);

        //ADJUST SHIELD SIZE ON ENTRY
        Weapon.ColliderAdjustment(.4f, 1.663404f);

        Weapon.OnWeaponStartSwing?.Invoke(shielder);
        Weapon.ClearEnemiesHitList();

        Weapon.SetPercentDamage(AttackPercentDamage);

        shielder.IsAttackAnimationPlaying = true;
        shielder.UseRootMotion = true;

    }

    public override void OnExit()
    {
        //ADJUST SHIELD SIZE ON ENTRY
        Weapon.ColliderAdjustment(.25f, 1.663404f);

        Weapon.OnWeaponEndSwing?.Invoke(shielder);
        shielder.IsAttackAnimationPlaying = false;
        shielder.UseRootMotion = false;
        shielder.EndHit();
    }

    public override void OnUpdate()
    {
        shielder.ApplyGravity();

        shielder.LookAt(shielder.transform.position + attackDirection);

        if (!shielder.IsAttackAnimationPlaying)
        {
            shielder.ChangeState(shielder.ShielderIdleState);
            return;
        }
    }
}


