using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemStompState : GolemBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float AOEInitialRadius { get; private set; } = 1f;
    [field: SerializeField] public float AOEPercentDamage { get; private set; } = 100f;
    [field: SerializeField] public float AOELaunchForce { get; private set; } = 7.5f;
    [field: SerializeField] public float AOEStunDuration { get; private set; } = 3f;
    [field: SerializeField] public float ShockwaveGrowMaxSteps { get; private set; } = 8f;
    [field: SerializeField] public float ShockwaveGrowStepDuration { get; private set; } = .05f;
    [field: SerializeField] public float ShockwaveGrowStepIncrement { get; private set; } = .25f;
    public Weapon Weapon { get; protected set; }

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        Weapon = entity.GetComponentInChildren<Weapon>();
    }

    public override void OnEnter() {
        golem.TransitionToAnimation("Stomp");
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

        golem.LookAt(golem.transform.position + golem.GolemReadyAttackState.GetAttackDirection());

        if (!golem.IsAttackAnimationPlaying)
        {
            golem.ChangeState(golem.GolemAttackRecoverState);
            return;
        }
    }

    public void GroundImpactShockwave() 
    {
        golem.ShakeCam();
        StartCoroutine(ShockwaveCoroutine());
    }

    private IEnumerator ShockwaveCoroutine() 
    {
        HashSet<Entity> entitiesHitByShockwave = new HashSet<Entity>();
        for (int i = 0; i < ShockwaveGrowMaxSteps; i++) 
        {
            // For each step, get hit entities within AOE
            List<Entity> entitiesInRadius = Entity.GetEntitiesThroughAOE(golem.transform.position, AOEInitialRadius +
                (i * ShockwaveGrowStepIncrement), false);
            CustomDebug.InstantiateTemporarySphere(golem.transform.position, AOEInitialRadius + (i * ShockwaveGrowStepIncrement), 0.25f, new Color(1f, 0, 0, 0.2f));

            foreach (Entity entityHit in entitiesInRadius) {
                if (entityHit == golem) continue;
                
                // To prevent damaging and launching the same target multiple times in a single shockwave, check if target has already been hit
                if (!entitiesHitByShockwave.Add(entityHit)) continue; // If entity already hit by shockwave, skip to next iteration. Else, add entity to entitiesHitByShockwave

                Vector3 direction = (entityHit.GetColliderCenterPosition() - golem.transform.position).normalized;
                entityHit.ForceChangeToLaunchState(direction, AOELaunchForce, AOEStunDuration);
                if (entityHit.Team == golem.Team) continue; // skip friendly entities, but still add them to hitEntities to launch them

                golem.DealDamageToOtherEntity(entityHit,
                        golem.CalculateDamage(AOEPercentDamage),
                        entityHit.CharacterController.ClosestPointOnBounds(golem.transform.position),
                        true);
                
            }
            
            yield return new WaitForSeconds(ShockwaveGrowStepDuration * golem.LocalTimeScale);


        }
        
       
      
        
        yield return null;
    }
    
    
    
    
    
}