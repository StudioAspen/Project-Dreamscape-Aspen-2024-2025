using UnityEngine;

public class ChargerJabbingAttackState : EnemyBaseState
{
    private Charger charger;

    private Entity rememberedTarget;

    public ChargerJabbingAttackState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        charger.IsAttackAnimationPlaying = true;
        charger.UseRootMotion = true;

        if (charger.RemainingJabs % 2 == 1)
        {
            charger.TransitionToAnimation("RightJab");

            charger.RightFistWeapon.OnWeaponStartSwing?.Invoke(charger);

            charger.RightFistWeapon.ClearEnemiesHitList();

            charger.RightFistWeapon.SetPercentDamage(charger.JabPercentDamage);
        }
        else
        {
            charger.TransitionToAnimation("LeftJab");

            charger.LeftFistWeapon.OnWeaponStartSwing?.Invoke(charger);

            charger.LeftFistWeapon.ClearEnemiesHitList();

            charger.LeftFistWeapon.SetPercentDamage(charger.JabPercentDamage);
        }
    }

    public override void OnExit()
    {
        charger.IsAttackAnimationPlaying = false;
        charger.UseRootMotion = false;

        if (charger.RemainingJabs % 2 == 1)
        {
            charger.RightFistWeapon.OnWeaponEndSwing?.Invoke(charger);
        }
        else
        {
            charger.LeftFistWeapon.OnWeaponEndSwing?.Invoke(charger);
        }

        charger.DisableWeaponTriggers();
    }

    public override void Update()
    {
        charger.ApplyGravity();

        if (rememberedTarget == null)
        {
            charger.ChangeState(charger.ChargerJabRecoverState);
            return;
        }

        charger.LookAt(rememberedTarget.transform.position, charger.JabRotationSpeed);

        // blocks update until attack animation is done
        if (charger.IsAttackAnimationPlaying) return;

        if (charger.RemainingJabs <= 0)
        {
            charger.ChangeState(charger.ChargerJabRecoverState);
            return;
        }

        charger.ForceChangeState(charger.ChargerJabbingAttackState);
    }

    public override void FixedUpdate()
    {

    }
}
