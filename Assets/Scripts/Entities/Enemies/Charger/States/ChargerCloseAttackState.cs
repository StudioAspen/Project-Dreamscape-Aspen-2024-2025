using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerCloseAttackState : EnemyBaseState {
    private Charger charger;
    private Vector3 attackDir;

    public ChargerCloseAttackState(Charger enemy) : base(enemy) {
        charger = enemy;
    }

    public void SetAttackDirection(Vector3 dir) {
        attackDir = dir;
    }

    public override void OnEnter() {
        charger.IsInterrupted = false;
        charger.LookAt(charger.transform.position + attackDir);
    }
    public override void OnExit() { }

    public override void Update() {
        if (charger.IsInterrupted) {
            charger.ChangeState(charger.ChargerIdleState);
            return;
        } else
            charger.ChangeState(charger.ChargerFarAttackState);
    }

    public override void FixedUpdate() { }
}