using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerCloseAttackState : EnemyBaseState
{
    private Charger charger;
    private Vector3 attackDir;

    private float closeAttackTimer;

    public ChargerCloseAttackState(Charger enemy):base(enemy)
    {
        charger = enemy;
    }

    public void SetAttackDirection(Vector3 dir)
    {
        attackDir = dir;
    }

    public override void OnEnter()
    {
        closeAttackTimer = 0;
        charger.IsInterrupted = false;
        charger.LookAt(charger.transform.position + attackDir);
    }
    public override void OnExit()
    {}

    public override void Update()
    {
        closeAttackTimer += Time.deltaTime;
        if (charger.IsHit)
        {
            charger.IsHit = false;
            Debug.Log("close attack to dazed");
            charger.ChangeState(charger.ChargerDazedState);
            return;
        }
        else if (closeAttackTimer >= 4)
        {
            charger.ChangeState(charger.ChargerChaseState);
            return;
        }
    }

    public override void FixedUpdate()
    {}
}
