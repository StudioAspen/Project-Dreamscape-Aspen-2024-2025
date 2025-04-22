
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class LeaperChaseState : EnemyChaseState
{
    [field: SerializeField] public float ChaseHopHeight { get; private set; } = 1.25f;
    [field: SerializeField] public float ChaseHopDistance { get; private set; } = 2f;
    [field: SerializeField] public float StartReadyAttackDistance { get; private set; } = 2f;

    private Leaper leaper; // Reference to the specific Leaper enemy using this state

    private Entity rememberedTarget;

    private Vector3 currentHopDestination;
    private Vector3 directionToHopDestination;

    public override void Init(Entity entity)
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

    public override void OnUpdate()
    {
        leaper.ApplyGravity();

        if(leaper.IsGrounded)
        {
            if (rememberedTarget == null)
            {
                leaper.ChangeState(leaper.LeaperWanderState);
                return;
            }

            if (leaper.Distance(rememberedTarget) < StartReadyAttackDistance)
            {
                leaper.LeaperReadyAttackState.AssignCurrentRememberedTarget(rememberedTarget);
                leaper.ChangeState(leaper.LeaperReadyAttackState);
                return;
            }

            currentHopDestination = GetCurrentHopDestination();
            directionToHopDestination = (currentHopDestination - leaper.transform.position).normalized;

            leaper.Hop(currentHopDestination, ChaseHopHeight);

            leaper.PlayOneShotAnimation(leaper.JumpAnimationClip);
        }

        leaper.LookAt(leaper.transform.position + directionToHopDestination);

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
        Vector3 currentHopDestination = ChaseHopDistance * direction + leaper.transform.position;

        return leaper.IsValidPointOnNavMesh(currentHopDestination, ChaseHopHeight, out Vector3 validDestination) ? validDestination : leaper.transform.position;
    }
}