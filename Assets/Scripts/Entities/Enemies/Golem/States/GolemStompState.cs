using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GolemStompState : GolemBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float AOEInitialRadius { get; private set; } = 1f;
    [field: SerializeField] public float AOEDamageMultiplier { get; private set; } = 1f;
    [field: SerializeField] public float AOELaunchForce { get; private set; } = 7.5f;
    [field: SerializeField] public float AOEStunDuration { get; private set; } = 3f;
    [field: SerializeField] public int ShockwaveGrowMaxSteps { get; private set; } = 8;
    [field: SerializeField] public float ShockwaveGrowStepDuration { get; private set; } = .05f;
    [field: SerializeField] public float ShockwaveRadiusGrowStepIncrement { get; private set; } = .25f;

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
        if (timer > duration)
        {
            golem.ChangeState(golem.GolemAttackRecoverState);
            return;
        }
    }

    public void GroundImpactShockwave() 
    {
        CameraShakeManager.Instance.ShakeCamera(5f, 1f, ShockwaveGrowMaxSteps * ShockwaveGrowStepDuration);
        golem.StartCoroutine(ShockwaveCoroutine());
    }

    private IEnumerator ShockwaveCoroutine() 
    {
        HashSet<Entity> entitiesHitByShockwave = new HashSet<Entity>();
        for (int i = 0; i < ShockwaveGrowMaxSteps; i++) 
        {
            // For each step, get hit entities within AOE
            List<Entity> entitiesInRadius = Entity.GetEntitiesThroughAOE(golem.transform.position, AOEInitialRadius +
                (i * ShockwaveRadiusGrowStepIncrement), false);
            CustomDebug.InstantiateTemporarySphere(golem.transform.position, AOEInitialRadius + (i * ShockwaveRadiusGrowStepIncrement), 0.25f, new Color(1f, 0, 0, 0.2f));

            foreach (Entity entityHit in entitiesInRadius)
            {
                if (entityHit == golem) continue;
                
                // To prevent damaging and launching the same target multiple times in a single shockwave, check if target has already been hit
                if (!entitiesHitByShockwave.Add(entityHit)) continue; // If entity already hit by shockwave, skip to next iteration. Else, add entity to entitiesHitByShockwave

                Vector3 direction = (entityHit.GetColliderCenterPosition() - golem.transform.position).normalized;
                entityHit.TryChangeToLaunchState(direction, AOELaunchForce, AOEStunDuration);
                if (entityHit.Team == golem.Team) continue; // skip friendly entities, but still add them to hitEntities to launch them

                golem.DealDamageToOtherEntity(entityHit,
                        golem.CalculateDamage(AOEDamageMultiplier),
                        entityHit.CharacterController.ClosestPointOnBounds(golem.transform.position),
                        true);
                
            }
            yield return new WaitForSeconds(ShockwaveGrowStepDuration / golem.LocalTimeScale.GetFloatValue());
        }
        yield return null;
    }
}