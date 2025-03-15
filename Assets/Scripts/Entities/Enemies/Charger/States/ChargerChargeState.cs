using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ChargerChargeState : ChargerBaseState
{
    [field: SerializeField] public float ChargeContactDamageMultiplier { get; private set; } = 2f;
    [field: SerializeField] public float ChargeSpeedModifier { get; private set; } = 5f;
    [field: SerializeField] public float ChargeDuration { get; private set; } = 20f;
    [field: SerializeField] public float ChargeRotationSpeed { get; private set; } = 5f;
    [field: SerializeField] public float ChargeOnImpactLaunchForce { get; private set; } = 10f;
    [field: SerializeField] public float ChargeStunDuration { get; private set; } = 4f;
    [field: SerializeField] public LayerMask ChargeLayerMask { get; private set; }

    private Entity rememberedTarget;

    private float timer;

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter() 
    {
        charger.PlayDefaultAnimation();
        
        charger.SetSpeedModifier(ChargeSpeedModifier);

        timer = 0f;
    }

    public override void OnExit() 
    {
        
    }

    public override void OnUpdate()
    {
        charger.ApplyGravity();

        CheckCollisions();

        if (rememberedTarget == null)
        {
            charger.ChangeState(charger.ChargerWindDownState);
            return;
        }

        timer += charger.LocalDeltaTime;
        if(timer > ChargeDuration)
        {
            charger.ChangeState(charger.ChargerWindDownState);
            return;
        }

        charger.UpdateHorizontalVelocity(charger.transform.forward);
        charger.ApplyHorizontalVelocity();

        charger.LookAt(rememberedTarget.transform.position, ChargeRotationSpeed);
    }

    /// <summary>
    /// Checks for collisions during the charger's charge state.
    /// </summary>
    private void CheckCollisions()
    {
        List<Collider> orderedHits = charger.GetCustomCollisionHits(ChargeLayerMask);

        foreach (Collider hit in orderedHits)
        {
            if (charger.DidHitWall(hit))
            {
                CameraShakeManager.Instance.ShakeCamera(3f, 1f, 0.5f);

                charger.ChangeState(charger.ChargerDazedState);
                return;
            }

            if (charger.DidHitFriendlyEntity(hit, out Entity friendlyEntity))
            {
                CameraShakeManager.Instance.ShakeCamera(2f, 1f,0.25f);

                Vector3 launchDirection = friendlyEntity.GetColliderCenterPosition() - charger.transform.position;
                friendlyEntity.TryChangeToLaunchState(charger, launchDirection, ChargeOnImpactLaunchForce, ChargeStunDuration);
            }

            if (charger.DidHitEnemyEntity(hit, out Entity enemyEntity))
            {
                CameraShakeManager.Instance.ShakeCamera(2f, 1f, 0.25f);

                Vector3 launchDirection = enemyEntity.GetColliderCenterPosition() - charger.transform.position;
                enemyEntity.TryChangeToLaunchState(charger, launchDirection, ChargeOnImpactLaunchForce, ChargeStunDuration);

                charger.DealDamageToOtherEntity(enemyEntity, charger.CalculateDamage(ChargeContactDamageMultiplier), hit.ClosestPoint(charger.GetColliderCenterPosition()), false);

                charger.ChangeState(charger.ChargerWindDownState);
                return;
            }
        }
    }
}