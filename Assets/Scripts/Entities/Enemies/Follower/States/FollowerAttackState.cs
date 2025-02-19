using UnityEngine;

public class FollowerAttackState : FollowerBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;
    [field: SerializeField] public float AttackDamageMultiplier { get; private set; } = 1f;
    public Weapon Weapon { get; protected set; }

    private Vector3 attackDirection;

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
        follower.PlayOneShotAnimation(AnimationClip);

        follower.SetSpeedModifier(0f);

        Weapon.OnWeaponStartSwing?.Invoke(follower);
        Weapon.ClearEnemiesHitList();

        Weapon.SetDamageMultiplier(AttackDamageMultiplier);

        follower.IsAttackAnimationPlaying = true;
        follower.UseRootMotion = true;
    }

    public override void OnExit()
    {
        Weapon.OnWeaponEndSwing?.Invoke(follower);
        follower.IsAttackAnimationPlaying = false;
        follower.UseRootMotion = false;
        follower.EndHit();
    }

    public override void OnUpdate()
    {
        follower.ApplyGravity();

        follower.LookAt(follower.transform.position + attackDirection);

        if (!follower.IsAttackAnimationPlaying)
        {
            follower.ChangeState(follower.FollowerAttackRecoverState);
            return;
        }
    }
}
