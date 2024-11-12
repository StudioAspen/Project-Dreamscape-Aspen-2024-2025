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
            charger.DefaultTransitionToAnimation("RightJab");

            charger.RightFistWeapon.OnWeaponStartSwing?.Invoke(charger);

            charger.RightFistWeapon.ClearEnemiesHitList();

            charger.RightFistWeapon.SetDamageRange(new Vector2Int(charger.JabDamageRange.x, charger.JabDamageRange.y));
        }
        else
        {
            charger.DefaultTransitionToAnimation("LeftJab");

            charger.LeftFistWeapon.OnWeaponStartSwing?.Invoke(charger);

            charger.LeftFistWeapon.ClearEnemiesHitList();

            charger.LeftFistWeapon.SetDamageRange(new Vector2Int(charger.JabDamageRange.x, charger.JabDamageRange.y));
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
        charger.LookAt(rememberedTarget.transform.position);

        // blocks update until attack animation is done
        if (charger.IsAttackAnimationPlaying) return;

        if (charger.RemainingJabs <= 0)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }

        charger.ForceChangeState(charger.ChargerJabbingAttackState);
    }

    public override void FixedUpdate()
    {

    }
}
