using UnityEngine;

[System.Serializable]
public class FollowerAttackState : FollowerBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float AttackDuration { get; private set; } = 1f;
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;
    [field: SerializeField] public float AttackDamageMultiplier { get; private set; } = 1f;
    public Weapon Weapon { get; protected set; }

    private Vector3 attackDirection;

    private float timer;

    public override void Init(Entity entity)
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
        follower.PlayOneShotAnimation(AnimationClip, AttackDuration);

        follower.SetSpeedModifier(0f);

        Weapon.OnWeaponStartSwing?.Invoke(follower, null);
        Weapon.ClearObjectHitList();

        Weapon.SetDamageMultiplier(AttackDamageMultiplier);

        follower.UseRootMotion = true;

        timer = 0f;
    }

    public override void OnExit()
    {
        Weapon.OnWeaponEndSwing?.Invoke(follower, null);
        follower.UseRootMotion = false;
        follower.EndHit();
    }

    public override void OnUpdate()
    {
        follower.ApplyGravity();

        follower.LookAt(follower.transform.position + attackDirection);

        timer += follower.LocalDeltaTime;
        if (timer > AttackDuration)
        {
            follower.ChangeState(follower.FollowerAttackRecoverState);
            return;
        }
    }
}
