using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaperAttackState : LeaperBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public LayerMask LeapAttackLayerMask { get; private set; }
    [field: SerializeField] public float LeapAttackHeightToDistanceRatio { get; private set; } = 0.2f;
    [field: SerializeField] public float AttackContactDamagePercent { get; private set; } = 150f;
    [field: SerializeField] public float RegularContactDamagePercent { get; private set; } = 100f;

    private Entity rememberedTarget;
    private Vector3 hopDestination;
    private Vector3 hopDirection;

    private List<Entity> entitiesHitByCurrentLeap = new List<Entity>();

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

        float hopHeight = leaper.Distance(rememberedTarget) * LeapAttackHeightToDistanceRatio;

        // Calculate the predicted movement of the remembered target (Speed * Time * Direction)
        Vector3 predictedMovement = 
            rememberedTarget.LocalTimeScale * rememberedTarget.MovementSpeed // speed of the target
            * leaper.CalculateHopDuration(rememberedTarget.transform.position, hopHeight) // time it takes to get to target
            * rememberedTarget.transform.forward; // direction of the target

        hopDestination = rememberedTarget.GetColliderCenterPosition() + predictedMovement;

        hopDirection = (hopDestination - leaper.transform.position).normalized;

        leaper.Hop(hopDestination, hopHeight);

        entitiesHitByCurrentLeap.Clear();
    }

    public override void OnExit()
    {
        rememberedTarget = null;
    }

    public override void OnUpdate()
    {
        leaper.ApplyGravity();

        if (leaper.IsGrounded)
        {
            leaper.ChangeState(leaper.LeaperWanderState);
            return;
        }

        leaper.LookAt(leaper.transform.position + hopDirection);

        if (!leaper.IsGrounded)
        {
            leaper.ApplyHorizontalVelocity();

            leaper.CheckCollisions(AttackContactDamagePercent, ref entitiesHitByCurrentLeap);
        }
    }
}