using UnityEngine;

public class GolemGroundSmashState : GolemBaseState
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
        golem.TransitionToAnimation("GolemGroundSmash");

        golem.SetSpeedModifier(0f);

        Weapon.OnWeaponStartSwing?.Invoke(golem);
        Weapon.ClearEnemiesHitList();

        Weapon.SetPercentDamage(AttackPercentDamage);

        golem.IsAttackAnimationPlaying = true;
        golem.UseRootMotion = true;
    }

    public override void OnExit()
    {
        Weapon.OnWeaponEndSwing?.Invoke(golem);
        golem.IsAttackAnimationPlaying = false;
        golem.UseRootMotion = false;
        golem.EndHit();
    }

    public override void OnUpdate()
    {
        golem.ApplyGravity();

        golem.LookAt(golem.transform.position + attackDirection);

        if (!golem.IsAttackAnimationPlaying)
        {
            golem.ChangeState(golem.GolemAttackRecoverState);
            return;
        }
    }
}