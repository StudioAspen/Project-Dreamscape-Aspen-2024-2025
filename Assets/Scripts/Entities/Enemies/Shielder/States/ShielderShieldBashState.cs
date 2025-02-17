using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShielderShieldBashState : ShielderBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;
    [field: SerializeField] public float AttackPercentDamage { get; private set; } = 0f;
    [SerializeField] private float TransitionDelay = 1.0f;


    [field: SerializeField] public Weapon Shield { get; protected set; }
    private List<Entity> entitiesHitByCurrentBash = new List<Entity>();


    private Vector3 attackDirection;
    private Entity rememberedTarget;

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }


    private protected override void Init(Entity entity)
    {
        base.Init(entity);
    }

    public void SetAttackDirection(Vector3 direction)
    {
        attackDirection = direction;
    }

    public override void OnEnter()
    {
        shielder.TransitionToAnimation("ShieldBash");
        shielder.SetSpeedModifier(0f);

        //ADJUST SHIELD SIZE ON ENTRY
        Shield.ColliderAdjustment(.8f, 1.6f);

        Shield.OnWeaponStartSwing?.Invoke(shielder);
        Shield.ClearEnemiesHitList();
        Shield.SetPercentDamage(AttackPercentDamage);

        entitiesHitByCurrentBash.Clear();
        shielder.CheckCollisions(0, ref entitiesHitByCurrentBash);

        shielder.IsAttackAnimationPlaying = true;
        shielder.UseRootMotion = true;

    }

    public override void OnExit()
    {
        //ADJUST SHIELD SIZE ON EXIT
        Shield.ColliderAdjustment(.8f, 1.6f);

        shielder.IsAttackAnimationPlaying = false;
        shielder.UseRootMotion = false;

        Shield.OnWeaponEndSwing?.Invoke(shielder);

        shielder.EndHit();
    }

    public override void OnUpdate()
    {

        shielder.LookAt(shielder.transform.position + attackDirection);

        if (!shielder.IsAttackAnimationPlaying)
        {
            StartCoroutine(Transition());
            return;
        }
    }

    private IEnumerator Transition()
    {
        yield return new WaitForSeconds(TransitionDelay);
        shielder.ShielderPowerAttackState.SetAttackDirection(attackDirection);
        shielder.ChangeState(shielder.ShielderPowerAttackState);

    }

}


