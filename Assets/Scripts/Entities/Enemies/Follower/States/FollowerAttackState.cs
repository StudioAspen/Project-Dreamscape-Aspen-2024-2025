using UnityEngine;

public class FollowerAttackState : FollowerBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;
    [field: SerializeField] public float AttackPercentDamage { get; private set; } = 100f;
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
        follower.TransitionToAnimation("Attack");

        follower.SetSpeedModifier(0f);

        Weapon.OnWeaponStartSwing?.Invoke(follower);
        Weapon.ClearEnemiesHitList();

        Weapon.SetPercentDamage(AttackPercentDamage);

        follower.IsAttackAnimationPlaying = true;
        follower.UseRootMotion = true;
    }

    public override void OnExit()
    {
        Weapon.OnWeaponEndSwing?.Invoke(follower);
        follower.IsAttackAnimationPlaying = false;
        follower.UseRootMotion = false;
        follower.DisableWeaponTriggers();
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
