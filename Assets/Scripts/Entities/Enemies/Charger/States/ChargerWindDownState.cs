using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChargerWindDownState : EnemyBaseState
{
    private Charger charger;

    private float timer;
    private float halfWindDownDuration;

    public ChargerWindDownState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        charger.TransitionToAnimation("FlatMovement");

        timer = 0f;
        halfWindDownDuration = charger.WindDownDuration / 2f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        timer += charger.LocalDeltaTime;

        if (timer > charger.WindDownDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }

        if(timer < halfWindDownDuration)
        {
            float easedSpeedModifier = DOVirtual.EasedValue(charger.ChargeSpeedModifier, 0, timer / halfWindDownDuration, Ease.OutQuad);
            charger.SetSpeedModifier(easedSpeedModifier);

            CheckCollisions();
        }
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

        foreach (Collider hit in orderedHits)
        {
            if (IsOwnDamageableEntityCollider(hit)) continue;

            if (DidChargerHitWall(hit))
            {
                charger.ChangeState(charger.ChargerDazedState);
                return;
            }
        }
    }

    private bool IsOwnDamageableEntityCollider(Collider hit)
    {
        // check if hit is a child of charger's collider
        Charger selfCharger = hit.GetComponentInParent<Charger>();

        if (selfCharger == null) return false;
        if (selfCharger == charger) return true;

        return false;
    }

    private bool DidChargerHitWall(Collider hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Hit a wall...");
            return true;
        }

        return false;
    }
}
