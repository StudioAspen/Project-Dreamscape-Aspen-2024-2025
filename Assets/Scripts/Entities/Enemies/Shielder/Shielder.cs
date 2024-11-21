using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : Enemy
{
   

    #region States
    public ShielderPatrolState ShielderPatrolState { get; private set; }
    public ShielderPlayerDetectedState ShielderPlayerDetectedState { get; private set; }

    [field: Header("Shielder: Wander Settings")]
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(3f, 5f);
    

    [field: Header("Shielder: Player Detected Settings")]
    [field: SerializeField] public float PlayerDetectedDuration { get; private set; } = 2f;
    [field: SerializeField] public float DetectionDistance { get; private set; } = 10f;
    [field: SerializeField] public float DetectionConeHalfAngle { get; private set; } = 30f;
    private float shielderEyesYOffset = -.3f;
    public Vector3 DetectionConeTopPoint => GetColliderCenterPosition() + (Vector3.up * (capsuleCollider.height / 2f)) + (Vector3.up * shielderEyesYOffset);

    protected override void InitializeStates()
    {
        base.InitializeStates();

        ShielderPatrolState = new ShielderPatrolState(this);
        ShielderPlayerDetectedState = new ShielderPlayerDetectedState(this);
    }

    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();
        SetStartState(ShielderPatrolState);
    }

    protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    protected override void OnOnAnimatorMove()
    {
        base.OnOnAnimatorMove();
    }

    protected override void OnStart()
    {
        base.OnStart();
        SetDefaultState(ShielderPatrolState);
        FinishAnimation();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public void FinishAnimation()
    {
        IsAttackAnimationPlaying = false;
       
    }

    public override void TryAssignTarget()
    {
        List<Entity> smallRadiusTargets = GetNearbyTargets();
        List<Entity> largeRadiusTargets = GetNearbyEntities(DetectionDistance);
        List<Entity> filteredTargetsByCone = FilterTargetsInConeShape(largeRadiusTargets, DetectionConeTopPoint, DetectionConeHalfAngle);

        if (largeRadiusTargets.Count == 0)
        {
            Target = null;
            return;
        }

        if (filteredTargetsByCone.Count > 0)
        {
            Target = filteredTargetsByCone[0];
            return;
        }

        if (smallRadiusTargets.Count > 0)
        {
            Target = smallRadiusTargets[0];
            return;
        }

        Target = null;
        return;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        CustomGizmos.DrawWireCircle(transform.position, targetDetectionRadius);
        CustomGizmos.DrawWireCone(DetectionConeTopPoint, transform.forward, DetectionConeHalfAngle, DetectionDistance);
        Gizmos.color = Color.white;
       
    }

}
