using UnityEngine;
using System.Collections;

[System.Serializable]
public class LeaperReadyAttackState : LeaperBaseState
{
    [field: SerializeField] public int ReadyAttackHopCount { get; private set; } = 2;
    [field: SerializeField] public float ReadyAttackHopDistance { get; private set; } = 1.5f;
    [field: SerializeField] public float ReadyAttackHopHeight { get; private set; } = .75f;
    [field: SerializeField] public float ReadyAttackStartDelay { get; private set; } = 0.75f;

    private Entity rememberedTarget;

    private int currentHopCount;

    private float attackStartDelayTimer;

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
        leaper.PlayDefaultAnimation();

        leaper.SetSpeedModifier(0);

        currentHopCount = ReadyAttackHopCount;

        attackStartDelayTimer = 0f;
    }

    public override void OnExit()
    {
        rememberedTarget = null;
    }

    public override void OnUpdate()
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
                if(attackStartDelayTimer > ReadyAttackStartDelay) TransitionToNextState();
                return;
            }

            Vector3 currentHopDestination = -ReadyAttackHopDistance * directionToTarget + leaper.transform.position;

            leaper.Hop(currentHopDestination, ReadyAttackHopHeight);

            leaper.PlayOneShotAnimation(leaper.JumpAnimationClip);

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