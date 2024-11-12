using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
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

    public override void OnEnter() //when state is switched to.
    {
        charger.DefaultTransitionToAnimation("FlatMovement");
        
        charger.SetSpeedModifier(5f);
        charger.SetRotationSpeed(charger.ChargeRotationSpeed);

        timer = 0f;
    }

    public override void OnExit() //when state is switched out.
    {

    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if(timer > charger.ChargeDuration)
        {
            // change to wind down state
            return;
        }

        charger.LookAt(rememberedTarget.transform.position);
    }

    public override void FixedUpdate()
    {
        charger.Move(charger.transform.forward);
    }

    private void CheckCollisions()
    {
        Collider[] hits = Physics.OverlapSphere(charger.GetColliderCenterPosition(), charger.ChargeCollisionRadius, charger.ChargeLayerMask);
    }
}