using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlimeChaseState : EnemyChaseState
{
    [field: SerializeField] public float ChaseHopHeight { get; private set; } = 1.25f;
    [field: SerializeField] public float ChaseHopDistance { get; private set; } = 2f;
    [field: SerializeField] public float StartAttackDistance { get; private set; } = 1f;

    private Slime slime;

    private Entity rememberedTarget;

    private Vector3 currentHopDestination;
    private Vector3 directionToHopDestination;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        slime = entity as Slime;
    }

    public void AssignCurentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        slime.SetSpeedModifier(0f);
    }

    public override void OnExit()
    {
        base.OnExit();

        rememberedTarget = null;
    }

    public override void OnUpdate()
    {
        slime.ApplyGravity();

        if(slime.IsGrounded)
        {
            if(rememberedTarget == null)
            {
                slime.ChangeState(slime.SlimeWanderState);
                return;
            }

            if (slime.Distance(rememberedTarget) < StartAttackDistance)
            {
                slime.SlimeAttackExpandState.AssignCurrentRememberedTarget(rememberedTarget);
                slime.ChangeState(slime.SlimeAttackExpandState);
                return;
            }

            currentHopDestination = GetCurrentHopDestination();
            directionToHopDestination = (currentHopDestination - slime.transform.position).normalized;

            slime.Hop(currentHopDestination, ChaseHopHeight);
        }

        slime.LookAt(slime.transform.position + directionToHopDestination);

        if (!slime.IsGrounded)
        {
            slime.ApplyHorizontalVelocity();
        }
    }
    
    private Vector3 GetCurrentHopDestination()
    {
        List<Vector3> path = slime.GetPathToDestination(rememberedTarget.transform.position);
        if(path == null) return slime.transform.position;
        if (path.Count < 2) return slime.transform.position;

        Vector3 currentDestination = path[1];

        Vector3 direction = (currentDestination - slime.transform.position).normalized;
        Vector3 currentHopDestination = ChaseHopDistance * direction + slime.transform.position;

        return slime.IsValidPointOnNavMesh(currentHopDestination, ChaseHopHeight, out Vector3 validDestination) ? currentHopDestination : slime.transform.position;
    }
}
