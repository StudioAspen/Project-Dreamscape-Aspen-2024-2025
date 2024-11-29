using UnityEngine;

public class FollowerAttackState : EnemyBaseState
{
    private Follower follower;

    private Vector3 attackDir;

    public FollowerAttackState(Follower enemy) : base(enemy)
    {
        follower = enemy;
    }

    public void SetAttackDirection(Vector3 dir)
    {
        attackDir = dir;
    }

    public override void OnEnter()
    {
        follower.TransitionToAnimation("Attack");

        follower.SetSpeedModifier(0f);

        follower.Weapon.OnWeaponStartSwing?.Invoke(follower);
        follower.Weapon.ClearEnemiesHitList();

        follower.Weapon.SetPercentDamage(follower.AttackPercentDamage);

        follower.IsAttackAnimationPlaying = true;
        follower.UseRootMotion = true;
    }

    public override void OnExit()
    {
        follower.Weapon.OnWeaponEndSwing?.Invoke(follower);
        follower.IsAttackAnimationPlaying = false;
        follower.UseRootMotion = false;
        follower.DisableWeaponTriggers();
    }

    public override void Update()
    {
        follower.LookAt(follower.transform.position + attackDir);

        if (!follower.IsAttackAnimationPlaying)
        {
            follower.ChangeState(follower.FollowerAttackRecoverState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        follower.ApplyGravity();
    }
}
