using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class ChargerChargeState : EnemyBaseState
{
    private Charger charger;

    private Entity rememberedTarget;

    private float timer;

    public ChargerChargeState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter() 
    {
        charger.TransitionToAnimation("FlatMovement");
        
        charger.SetSpeedModifier(charger.ChargeSpeedModifier);
        charger.SetRotationSpeed(charger.ChargeRotationSpeed);

        timer = 0f;
    }

    public override void OnExit() 
    {
        charger.ResetRotationSpeed();
    }

    public override void Update()
    {
        if(rememberedTarget == null)
        {
            charger.ChangeState(charger.ChargerWindDownState);
            return;
        }

        timer += Time.deltaTime;
        if(timer > charger.ChargeDuration)
        {
            charger.ChangeState(charger.ChargerWindDownState);
            return;
        }

        CheckCollisions();

        charger.LookAt(rememberedTarget.transform.position);
    }

    public override void FixedUpdate()
    {
        charger.Move(charger.transform.forward);
    }

    private void CheckCollisions()
    {
        // charge layer mask should only be ground and damageable entities
        Collider[] hits = Physics.OverlapCapsule(charger.ChargeCollisionBottomPoint, charger.ChargeCollisionTopPoint, charger.ChargeCollisionRadius, charger.ChargeLayerMask);

        if (hits == null) return;
        if (hits.Length == 0) return;

        List<Collider> orderedHits = hits.OrderBy(hit => charger.Distance(hit.ClosestPoint(charger.GetColliderCenterPosition()))).ToList();

        foreach(Collider hit in orderedHits)
        {
            if(IsOwnDamageableEntityCollider(hit)) continue;

            if(DidChargerHitWall(hit))
            {
                CameraShakeManager.Instance.ShakeCamera(3f, 0.5f);

                charger.ChangeState(charger.ChargerDazedState);
                return;
            }

            if(DidChargerHitFriendlyEntity(hit, out Entity friendlyEntity))
            {
                CameraShakeManager.Instance.ShakeCamera(2f, 0.25f);

                Vector3 flingDirection = friendlyEntity.GetColliderCenterPosition() - charger.transform.position;
                TryFlingEntity(friendlyEntity, flingDirection, charger.ChargeFlingForce, charger.ChargeStunDuration);
            }

            if(DidChargerHitEnemyEntity(hit, out Entity enemyEntity))
            {
                CameraShakeManager.Instance.ShakeCamera(2f, 0.25f);

                Vector3 flingDirection = enemyEntity.GetColliderCenterPosition() - charger.transform.position;
                TryFlingEntity(enemyEntity, flingDirection, charger.ChargeFlingForce, charger.ChargeStunDuration);

                enemyEntity.TakeDamageWithoutState(charger.CalculateDamage(charger.ChargeContactPercentDamage), hit.ClosestPoint(charger.GetColliderCenterPosition()), charger.gameObject);

                charger.ChangeState(charger.ChargerWindDownState);
                return;
            }
        }
    }

    private bool IsOwnDamageableEntityCollider(Collider hit)
    {
        // check if hit is a child of charger's collider
        Charger selfCharger = hit.GetComponentInParent<Charger>();

        if(selfCharger == null) return false;
        if (selfCharger == charger) return true;

        return false;
    }

    private bool DidChargerHitFriendlyEntity(Collider hit, out Entity entity)
    {
        entity = hit.GetComponentInParent<Entity>();

        if(entity == null) return false;
        if (entity.Team != charger.Team) return false;

        return true;
    }

    private bool DidChargerHitEnemyEntity(Collider hit, out Entity entity)
    {
        entity = hit.GetComponentInParent<Entity>();

        if (entity == null) return false;
        if (entity.Team == charger.Team) return false;

        return true;
    }

    private bool DidChargerHitWall(Collider hit)
    {
        return hit.gameObject.layer == LayerMask.NameToLayer("Ground");
    }

    private void TryFlingEntity(Entity entity, Vector3 direction, float force, float stunDuration)
    {
        //if (entity.GetType() == typeof(Charger)) return; // prevents chargers from flinging other chargers

        entity.TryChangeToLaunchState(direction, force, stunDuration);
    }
}