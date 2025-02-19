using UnityEngine;

public class ChargerJabbingAttackState : ChargerBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public AnimationClip RightJabAnimationClip { get; protected set; }
    [field: SerializeField] public AnimationClip LeftJabAnimationClip { get; protected set; }
    [field: SerializeField] public int JabCount { get; private set; } = 5;
    [field: SerializeField] public float JabDamageMultiplier { get; private set; } = 1f;
    [field: SerializeField] public float JabStandStillRadius { get; private set; } = 1.5f;
    [field: SerializeField] public float JabRotationSpeed { get; private set; } = 25f;
    [field: SerializeField] public Weapon LeftFistWeapon { get; private set; }
    [field: SerializeField] public Weapon RightFistWeapon { get; private set; }
    public int RemainingJabs { get; private set; }

    private Entity rememberedTarget;
    private Vector3 directionToTarget;

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
            charger.PlayOneShotAnimation(RightJabAnimationClip);

            RightFistWeapon.OnWeaponStartSwing?.Invoke(charger);

            RightFistWeapon.ClearEnemiesHitList();

            RightFistWeapon.SetDamageMultiplier(JabDamageMultiplier);
        }
        else
        {
            charger.PlayOneShotAnimation(LeftJabAnimationClip);

            LeftFistWeapon.OnWeaponStartSwing?.Invoke(charger);

            LeftFistWeapon.ClearEnemiesHitList();

            LeftFistWeapon.SetDamageMultiplier(JabDamageMultiplier);
        }

        directionToTarget = rememberedTarget.transform.position - charger.transform.position;
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

        charger.LookAt(charger.transform.position + directionToTarget, JabRotationSpeed);

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
