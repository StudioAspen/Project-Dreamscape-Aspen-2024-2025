using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ChargerChargeState : ChargerBaseState
{
    private Entity rememberedTarget;

    private float timer;

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter() 
    {
        charger.TransitionToAnimation("FlatMovement");
        
        charger.SetSpeedModifier(charger.ChargeSpeedModifier);

        timer = 0f;
    }

    public override void OnExit() 
    {
        
    }

    public override void OnUpdate()
    {
        charger.ApplyGravity();

        if (rememberedTarget == null)
        {
            charger.ChangeState(charger.ChargerWindDownState);
            return;
        }

        timer += charger.LocalDeltaTime;
        if(timer > charger.ChargeDuration)
        {
            charger.ChangeState(charger.ChargerWindDownState);
            return;
        }

        CheckCollisions();

        charger.UpdateHorizontalVelocity(charger.transform.forward);
        charger.ApplyHorizontalVelocity();

        charger.LookAt(rememberedTarget.transform.position, charger.ChargeRotationSpeed);
    }

    /// <summary>
    /// Checks for collisions during the charger's charge state.
    /// </summary>
    private void CheckCollisions()
    {
        List<Collider> orderedHits = charger.GetCustomCollisionHits(charger.ChargeLayerMask);

        foreach (Collider hit in orderedHits)
        {
            if (charger.DidHitWall(hit))
            {
                CameraShakeManager.Instance.ShakeCamera(3f, 0.5f);

                charger.ChangeState(charger.ChargerDazedState);
                return;
            }

            if (charger.DidHitFriendlyEntity(hit, out Entity friendlyEntity))
            {
                CameraShakeManager.Instance.ShakeCamera(2f, 0.25f);

                Vector3 launchDirection = friendlyEntity.GetColliderCenterPosition() - charger.transform.position;
                friendlyEntity.TryChangeToLaunchState(launchDirection, charger.ChargeOnImpactLaunchForce, charger.ChargeStunDuration);
            }

            if (charger.DidHitEnemyEntity(hit, out Entity enemyEntity))
            {
                CameraShakeManager.Instance.ShakeCamera(2f, 0.25f);

                Vector3 launchDirection = enemyEntity.GetColliderCenterPosition() - charger.transform.position;
                enemyEntity.TryChangeToLaunchState(launchDirection, charger.ChargeOnImpactLaunchForce, charger.ChargeStunDuration);

                enemyEntity.TakeDamage(charger.CalculateDamage(charger.ChargeContactPercentDamage), hit.ClosestPoint(charger.GetColliderCenterPosition()), charger.gameObject, false);

                charger.ChangeState(charger.ChargerWindDownState);
                return;
            }
        }
    }
}