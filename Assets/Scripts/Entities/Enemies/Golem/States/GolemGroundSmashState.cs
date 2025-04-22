using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GolemGroundSmashState : GolemBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float AOERadius { get; private set; } = 1f;
    [field: SerializeField] public float AOEDamageMultiplier { get; private set; } = 1f;
    [field: SerializeField] public float AOELaunchForce { get; private set; } = 7.5f;
    [field: SerializeField] public float AOEStunDuration { get; private set; } = 3f;

    private float timer;
    private float duration;

    public override void OnEnter()
    {
        golem.PlayOneShotAnimation(AnimationClip);
        golem.SetSpeedModifier(0f);
        
        golem.UseRootMotion = false;

        timer = 0f;
        duration = AnimationClip.length;
    }

    public override void OnExit()
    {
        golem.UseRootMotion = false;
    }

    public override void OnUpdate()
    {
        golem.ApplyGravity();
        golem.LookAt(golem.transform.position + golem.GolemReadyAttackState.GetAttackDirection());

        timer += golem.LocalDeltaTime;
        if(timer > duration)
        {
            golem.ChangeState(golem.GolemAttackRecoverState);
            return;
        }
    }

    public void GroundImpact()
    {
        CameraShakeManager.Instance.ShakeCamera(5f, 1f, 0.75f);
        GolemHitEntitiesWithAOEIgnoreTeam(golem.transform.position, AOERadius, AOEDamageMultiplier, AOELaunchForce, AOEStunDuration);
        CustomDebug.InstantiateTemporarySphere(golem.transform.position, AOERadius, 0.25f, new Color(1f, 0, 0, 0.2f));
    }
    
    /// <summary>
    /// Applies area of effect damage to enemy entities within a given radius. The AOE ignores the attacker.
    /// Launches all hit entities regardless of team, excluding the attacker.
    /// Modified/Combined version of DamageEnemyEntitiesWithAOELaunch and DamageEnemyEntitiesWithAOE
    /// Did not want to mess with Entity.cs, so I created a function here.
    /// </summary>
    /// <param name="center">The center position of the AOE.</param>
    /// <param name="radius">The radius within which entities will be damaged.</param>
    /// <param name="damageMultiplier">The damage multiplier to apply to each entity.</param>
    /// <param name="launchForce">The force with which to launch the entities within the AOE.</param>
    /// <param name="stunDuration">The duration of the stun effect applied to the entities within the AOE.</param>
    /// <param name="willTryStagger">Whether to try to stagger the entities hit.</param>
    /// <returns>A list of entities that were hit.</returns>
    public List<Entity> GolemHitEntitiesWithAOEIgnoreTeam(Vector3 center, float radius, float damageMultiplier, float launchForce, float stunDuration, bool willTryStagger = true)
    {
        List<Entity> entitiesInRadius = Entity.GetEntitiesThroughAOE(center, radius, false);
        List<Entity> entitiesHit = new List<Entity>();

        foreach (Entity entityHit in entitiesInRadius)
        {
            if (entityHit == golem) continue; // ignore self
            entitiesHit.Add(entityHit);
            Vector3 direction = (entityHit.GetColliderCenterPosition() - center).normalized;
            entityHit.TryChangeToLaunchState(golem, direction, launchForce, stunDuration);
            if (entityHit.Team == golem.Team) continue; // skip friendly entities, but still add them to hitEntities to launch them
            golem.DealDamageToOtherEntity(entityHit,
                golem.CalculateDamage(damageMultiplier),
                entityHit.CharacterController.ClosestPointOnBounds(center),
                willTryStagger);
        }
        return entitiesHit;
    } 
}