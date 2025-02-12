using System.Collections.Generic;
using UnityEngine;

public class GolemGroundSmashState : GolemBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float AOERadius { get; private set; } = 1f;
    [field: SerializeField] public float AOEPercentDamage { get; private set; } = 100f;
    [field: SerializeField] public float AOELaunchForce { get; private set; } = 7.5f;
    [field: SerializeField] public float AOEStunDuration { get; private set; } = 3f;
    public Weapon Weapon { get; protected set; }

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        Weapon = entity.GetComponentInChildren<Weapon>();
    }

    public override void OnEnter()
    {
        golem.TransitionToAnimation("GroundSmash");

        golem.SetSpeedModifier(0f);

        Weapon.OnWeaponStartSwing?.Invoke(golem);
        Weapon.ClearEnemiesHitList();
        
        golem.IsAttackAnimationPlaying = true;
        golem.UseRootMotion = false;
    }

    public override void OnExit()
    {
        Weapon.OnWeaponEndSwing?.Invoke(golem);
        golem.IsAttackAnimationPlaying = false;
        golem.UseRootMotion = false;
        golem.EndHit();
    }

    public override void OnUpdate()
    {
        golem.ApplyGravity();

        golem.LookAt(golem.GolemReadyAttackState.GetAttackDirection());

        if (!golem.IsAttackAnimationPlaying)
        {
            golem.ChangeState(golem.GolemAttackRecoverState);
            return;
        }
    }

    public void GroundImpact() {
        golem.ShakeCam();
        GolemHitEntitiesWithAOEIgnoreTeam(golem, golem.transform.position, AOERadius, AOEPercentDamage, AOELaunchForce, AOEStunDuration);
        CustomDebug.InstantiateTemporarySphere(golem.transform.position, AOERadius, 0.25f, new Color(1f, 0, 0, 0.2f));
    }
    
    
    /// <summary>
    /// Applies area of effect damage to enemy entities within a given radius. The AOE ignores the attacker.
    /// Launches all hit entities regardless of team, excluding the attacker.
    /// Modified/Combined version of DamageEnemyEntitiesWithAOELaunch and DamageEnemyEntitiesWithAOE
    /// Did not want to mess with Entity.cs, so I created a function here.
    /// </summary>
    /// <param name="attacker">The entity causing the damage.</param>
    /// <param name="center">The center position of the AOE.</param>
    /// <param name="radius">The radius within which entities will be damaged.</param>
    /// <param name="percentDamage">The percentage of damage to apply to each entity.</param>
    /// <param name="launchForce">The force with which to launch the entities within the AOE.</param>
    /// <param name="stunDuration">The duration of the stun effect applied to the entities within the AOE.</param>
    /// <param name="willTryStagger">Whether to try to stagger the entities hit.</param>
    /// <returns>A list of entities that were hit.</returns>
    public static List<Entity> GolemHitEntitiesWithAOEIgnoreTeam(Entity attacker, Vector3 center, float radius, float percentDamage, float launchForce, float stunDuration, bool willTryStagger = true)
    {
        List<Entity> entitiesInRadius = Entity.GetEntitiesThroughAOE(center, radius, false);
        List<Entity> entitiesHit = new List<Entity>();

        foreach (Entity entityHit in entitiesInRadius) {
            if (entityHit == attacker) continue;
            entitiesHit.Add(entityHit);
            Vector3 direction = (entityHit.GetColliderCenterPosition() - center).normalized;
            entityHit.ForceChangeToLaunchState(direction, launchForce, stunDuration);
            if (entityHit.Team == attacker.Team) continue; // skip friendly entities, but still add them to hitEntities to launch them
            attacker.DealDamageToOtherEntity(entityHit,
                attacker.CalculateDamage(percentDamage),
                entityHit.CharacterController.ClosestPointOnBounds(center),
                willTryStagger);
        }
        return entitiesHit;
    }
    
    
}