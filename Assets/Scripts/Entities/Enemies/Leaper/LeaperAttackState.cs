using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaperAttackState : EnemyBaseState
{
    private Leaper leaper;

    private Entity rememberedTarget;
    private Vector3 hopDestination;

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

        Vector3 predictedMovement = rememberedTarget.LocalTimeScale * rememberedTarget.MovementSpeed * leaper.AttackHopDuration * rememberedTarget.transform.forward;
        hopDestination = rememberedTarget.GetColliderCenterPosition() + predictedMovement;

        // 0 hop height, because duration overrides that
        leaper.Hop(hopDestination, 0f, leaper.AttackHopDuration);

        entitiesHitByCurrentLeap.Clear();
    }

    public override void OnExit()
    {
        rememberedTarget = null;
    }

    public override void Update()
    {
        leaper.ApplyGravity();

        if (leaper.IsGrounded)
        {
            leaper.ChangeState(leaper.LeaperWanderState);
            return;
        }

        leaper.LookAt(hopDestination);

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