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
        follower.DefaultTransitionToAnimation("Attack");

        follower.SetSpeedModifier(0f);

        follower.Weapon.OnWeaponStartSwing?.Invoke(follower);
        follower.Weapon.ClearEnemiesHitList();

        follower.Weapon.SetDamageRange(new Vector2Int(follower.AttackDamageRange.x, follower.AttackDamageRange.y));

        follower.IsAttackAnimationPlaying = true;
        follower.UseRootMotion = true;

        follower.LookAt(follower.transform.position + attackDir);
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
        if (!follower.IsAttackAnimationPlaying)
        {
            follower.ChangeState(follower.FollowerAttackRecoverState);
            return;
        }
    }

    public override void FixedUpdate()
    {

    }
}
