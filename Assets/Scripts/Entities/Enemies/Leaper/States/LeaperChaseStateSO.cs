
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LeaperChaseStateSO : EnemyChaseStateSO
{
    private Leaper leaper; // Reference to the specific Leaper enemy using this state

    private Entity rememberedTarget;

    private Vector3 currentHopDestination;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        leaper = entity as Leaper;
    }

    /// <summary>
    /// Assigns a remembered target entity to the Leaper enemy to lock onto.
    /// Must be called right before changing to its chase state.
    /// </summary>
    /// <param name="target">The entity to assign as the remembered target.</param>
    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        leaper.SetSpeedModifier(0f);
    }

    public override void OnExit()
    {
        base.OnExit();

        rememberedTarget = null;
    }

    public override void Update()
    {
        leaper.ApplyGravity();

        if(leaper.IsGrounded)
        {
            if (rememberedTarget == null)
            {
                leaper.ChangeState(leaper.LeaperWanderState);
                return;
            }

            if (leaper.Distance(rememberedTarget) < leaper.StartReadyAttackDistance)
            {
                leaper.LeaperReadyAttackState.AssignCurrentRememberedTarget(rememberedTarget);
                leaper.ChangeState(leaper.LeaperReadyAttackState);
                return;
            }

            currentHopDestination = GetCurrentHopDestination();

            leaper.Hop(currentHopDestination, leaper.ChaseHopHeight);

            leaper.TransitionToAnimation("JumpingUp");
        }

        leaper.LookAt(currentHopDestination);

        if (!leaper.IsGrounded)
        {
            leaper.ApplyHorizontalVelocity();
        }
    }

    /// <summary>
    /// Calculates the current hop destination for the Leaper enemy based on the remembered target entity.
    /// </summary>
    /// <returns>The current hop destination vector.</returns>
    private Vector3 GetCurrentHopDestination()
    {
        List<Vector3> path = leaper.GetPathToDestination(rememberedTarget.transform.position);
        if (path == null) return leaper.transform.position;
        if (path.Count < 2) return leaper.transform.position;

        Vector3 currentDestination = path[1];

        Vector3 direction = (currentDestination - leaper.transform.position).normalized;
        Vector3 currentHopDestination = leaper.ChaseHopDistance * direction + leaper.transform.position;

        return leaper.IsValidPointOnNavMesh(currentHopDestination, leaper.ChaseHopHeight, out Vector3 validDestination) ? validDestination : leaper.transform.position;
    }
}