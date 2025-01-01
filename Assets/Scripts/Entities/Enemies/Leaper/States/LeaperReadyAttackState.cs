using UnityEngine;
using System.Collections;

public class LeaperReadyAttackState : EnemyBaseState
{
    private Leaper leaper;

    private Entity rememberedTarget;

    private int currentHopCount;

    private float attackStartDelayTimer;

    public LeaperReadyAttackState(Leaper enemy) : base(enemy)
    {
        leaper = enemy;
    }

    /// <summary>
    /// Assigns a remembered target entity to the Leaper enemy to lock onto.
    /// Must be called right before changing to its ready attack state.
    /// </summary>
    /// <param name="target">The entity to assign as the remembered target.</param>
    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        leaper.TransitionToAnimation("FlatMovement");

        leaper.SetSpeedModifier(0);

        currentHopCount = leaper.ReadyAttackHopCount;

        attackStartDelayTimer = 0f;
    }

    public override void OnExit()
    {
        rememberedTarget = null;
    }

    public override void Update()
    {
        leaper.ApplyGravity();

        if(rememberedTarget == null)
        {
            leaper.ChangeState(leaper.LeaperWanderState);
            return;
        }

        if (currentHopCount <= 0) attackStartDelayTimer += leaper.LocalDeltaTime;

        Vector3 directionToTarget = (rememberedTarget.transform.position - leaper.transform.position).normalized;

        leaper.LookAt(rememberedTarget.transform.position);

        if (leaper.IsGrounded)
        {
            if(currentHopCount <= 0)
            {
                if(attackStartDelayTimer > leaper.ReadyAttackStartDelay) TransitionToNextState();
                return;
            }

            Vector3 currentHopDestination = -leaper.ReadyAttackHopDistance * directionToTarget + leaper.transform.position;

            leaper.Hop(currentHopDestination, leaper.ReadyAttackHopHeight);

            leaper.TransitionToAnimation("JumpingUp");

            currentHopCount--;
        }

        if (!leaper.IsGrounded)
        {
            leaper.ApplyHorizontalVelocity();
        }
    }

    public void TransitionToNextState()
    {
        bool willAttack = Random.Range(0, 2) == 1;

        if (willAttack)
        {
            leaper.LeaperAttackState.AssignCurrentRememberedTarget(rememberedTarget);
            leaper.ChangeState(leaper.LeaperAttackState);
        }
        else
        {
            leaper.LeaperChaseState.AssignCurrentRememberedTarget(rememberedTarget);
            leaper.ChangeState(leaper.EnemyChaseState);
        }
    }
}