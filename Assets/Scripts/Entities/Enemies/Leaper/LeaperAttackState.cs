using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaperAttackState : EnemyBaseState
{
    private Leaper leaper;

    private Entity rememberedTarget;

    private List<Entity> entitiesHitByCurrentLeap = new List<Entity>();

    private bool isAttackStarted;

    public LeaperAttackState(Leaper enemy) : base(enemy)
    {
        leaper = enemy;
    }

    /// <summary>
    /// Assigns a remembered target entity to the Leaper enemy to lock onto.
    /// Must be called right before changing to its attack state.
    /// </summary>
    /// <param name="target">The entity to assign as the remembered target.</param>
    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        leaper.TransitionToAnimation("JumpingUp");

        leaper.SetSpeedModifier(0f);

        Vector3 directionToTarget  = (rememberedTarget.transform.position - leaper.transform.position).normalized;
        // 0 hop height, because duration overrides that
        leaper.Hop(leaper.AttackHopOvershootDistance * directionToTarget + rememberedTarget.transform.position, 0f, leaper.AttackHopDuration);

        entitiesHitByCurrentLeap.Clear();
    }

    public override void OnExit()
    {
        rememberedTarget = null;
    }

    public override void Update()
    {
        leaper.ApplyGravity();

        if (rememberedTarget == null)
        {
            leaper.ChangeState(leaper.LeaperWanderState);
            return;
        }

        if (leaper.IsGrounded)
        {
            leaper.ChangeState(leaper.LeaperWanderState);
            return;
        }

        leaper.LookAt(rememberedTarget.transform.position);

        if (!leaper.IsGrounded)
        {
            leaper.ApplyHorizontalVelocity();

            leaper.CheckCollisions(leaper.AttackContactDamagePercent, ref entitiesHitByCurrentLeap);
        }
    }

    public override void FixedUpdate()
    {

    }
}