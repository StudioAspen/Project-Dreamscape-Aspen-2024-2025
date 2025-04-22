using UnityEngine;

[System.Serializable]
public class ChargerJabbingAttackState : ChargerBaseState
{
    [field: SerializeField] public AnimationClip RightJabAnimationClip { get; protected set; }
    [field: SerializeField] public AnimationClip LeftJabAnimationClip { get; protected set; }
    [field: SerializeField] public int JabCount { get; private set; } = 5;
    [field: SerializeField] public float JabDuration { get; private set; } = 0.45f;
    [field: SerializeField] public float JabDamageMultiplier { get; private set; } = 1f;
    [field: SerializeField] public float JabStandStillRadius { get; private set; } = 1.5f;
    [field: SerializeField] public float JabRotationSpeed { get; private set; } = 25f;
    [field: SerializeField] public Weapon LeftFistWeapon { get; private set; }
    [field: SerializeField] public Weapon RightFistWeapon { get; private set; }
    public int RemainingJabs { get; private set; }

    private Entity rememberedTarget;
    private Vector3 directionToTarget;
    private float timer;
    private bool leftMissEnabled = true;
    private bool rightMissEnabled = true;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        LeftFistWeapon.OnWeaponEndSwing += ConditionallyPlayLeftMiss;
        RightFistWeapon.OnWeaponEndSwing += ConditionallyPlayRightMiss;
        LeftFistWeapon.OnWeaponHit += PlayLeftHit;
        RightFistWeapon.OnWeaponHit += PlayRightHit;
    }

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        timer = 0f;

        charger.UseRootMotion = true;

        if (RemainingJabs % 2 == 1)
        {
            charger.PlayOneShotAnimation(RightJabAnimationClip, JabDuration);

            RightFistWeapon.OnWeaponStartSwing?.Invoke(charger, null);

            RightFistWeapon.ClearObjectHitList();

            RightFistWeapon.SetDamageMultiplier(JabDamageMultiplier);
        }
        else
        {
            charger.PlayOneShotAnimation(LeftJabAnimationClip, JabDuration);

            LeftFistWeapon.OnWeaponStartSwing?.Invoke(charger, null);

            LeftFistWeapon.ClearObjectHitList();

            LeftFistWeapon.SetDamageMultiplier(JabDamageMultiplier);
        }

        directionToTarget = rememberedTarget.transform.position - charger.transform.position;
    }

    public override void OnExit()
    {
        charger.UseRootMotion = false;

        if (RemainingJabs % 2 == 1)
        {
            RightFistWeapon.OnWeaponEndSwing?.Invoke(charger, null);
        }
        else
        {
            LeftFistWeapon.OnWeaponEndSwing?.Invoke(charger, null);
        }

        charger.EndHit();
    }

    public override void OnUpdate()
    {
        charger.ApplyGravity();

        timer += charger.LocalDeltaTime;

        if (rememberedTarget == null)
        {
            charger.ChangeState(charger.ChargerJabRecoverState);
            return;
        }

        charger.LookAt(charger.transform.position + directionToTarget, JabRotationSpeed);

        // blocks update until attack animation is done
        if (timer < JabDuration) return;

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

    public void PlayLeftHit(Entity entity, Entity target, Vector3 hitPoint, int damage)
    {
        AkSoundEngine.PostEvent("Play_ChargerHitLeft", charger.gameObject);
        leftMissEnabled = false;
    }

    public void PlayRightHit(Entity entity, Entity target, Vector3 hitPoint, int damage)
    {
        AkSoundEngine.PostEvent("Play_ChargerHitRight", charger.gameObject);
        rightMissEnabled = false;
    }

    public void ConditionallyPlayLeftMiss(Entity entity, ComboDataSO combo)
    {
        if (leftMissEnabled)
        {
            AkSoundEngine.PostEvent("Play_ChargerMissLeft", charger.gameObject);
        }
        leftMissEnabled = true;
    }

    public void ConditionallyPlayRightMiss(Entity entity, ComboDataSO combo)
    {
        if (rightMissEnabled)
        {
            AkSoundEngine.PostEvent("Play_ChargerMissRight", charger.gameObject);
        }
        rightMissEnabled = true;
    }
}
