using UnityEngine;

public class ChargerJabbingAttackState : ChargerBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public int JabCount { get; private set; } = 5;
    [field: SerializeField] public float JabPercentDamage { get; private set; } = 100;
    [field: SerializeField] public float JabStandStillRadius { get; private set; } = 1.5f;
    [field: SerializeField] public float JabRotationSpeed { get; private set; } = 25f;
    [field: SerializeField] public Weapon LeftFistWeapon { get; private set; }
    [field: SerializeField] public Weapon RightFistWeapon { get; private set; }
    public int RemainingJabs { get; private set; }

    private Entity rememberedTarget;

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        charger.IsAttackAnimationPlaying = true;
        charger.UseRootMotion = true;

        if (RemainingJabs % 2 == 1)
        {
            charger.TransitionToAnimation("RightJab");

            RightFistWeapon.OnWeaponStartSwing?.Invoke(charger);

            RightFistWeapon.ClearEnemiesHitList();

            RightFistWeapon.SetPercentDamage(JabPercentDamage);
        }
        else
        {
            charger.TransitionToAnimation("LeftJab");

            LeftFistWeapon.OnWeaponStartSwing?.Invoke(charger);

            LeftFistWeapon.ClearEnemiesHitList();

            LeftFistWeapon.SetPercentDamage(JabPercentDamage);
        }
    }

    public override void OnExit()
    {
        charger.IsAttackAnimationPlaying = false;
        charger.UseRootMotion = false;

        if (RemainingJabs % 2 == 1)
        {
            RightFistWeapon.OnWeaponEndSwing?.Invoke(charger);
        }
        else
        {
            LeftFistWeapon.OnWeaponEndSwing?.Invoke(charger);
        }

        charger.EndHit();
    }

    public override void OnUpdate()
    {
        charger.ApplyGravity();

        if (rememberedTarget == null)
        {
            charger.ChangeState(charger.ChargerJabRecoverState);
            return;
        }

        charger.LookAt(rememberedTarget.transform.position, JabRotationSpeed);

        // blocks update until attack animation is done
        if (charger.IsAttackAnimationPlaying) return;

        if (RemainingJabs <= 0)
        {
            charger.ChangeState(charger.ChargerJabRecoverState);
            return;
        }

        charger.ChangeState(charger.ChargerJabbingAttackState, true);
    }

    public void ResetJabCount()
    {
        RemainingJabs = JabCount;
    }

    public void DecrementJabCount()
    {
        RemainingJabs--;
    }
}
