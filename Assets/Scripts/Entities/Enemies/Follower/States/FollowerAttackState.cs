using UnityEngine;

public class FollowerAttackState : FollowerBaseState
{
    private Vector3 attackDirection;

    public void SetAttackDirection(Vector3 direction)
    {
        attackDirection = direction;
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
