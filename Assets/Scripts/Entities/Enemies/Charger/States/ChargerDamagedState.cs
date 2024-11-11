using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerDamagedState : EnemyBaseState {
    private Charger charger;
    private float damagedStateTimer;
    public ChargerDamagedState(Charger enemy) : base(enemy) {
        charger = enemy;
    }

    public override void OnEnter() {
        charger.IsDazed = true;
        damagedStateTimer = 0f;
    }
    public override void OnExit() { }

    public override void Update() {
        damagedStateTimer += Time.deltaTime;

        if (damagedStateTimer > charger.DamagedStateDuration) {
            charger.IsDazed = false;
            charger.ChangeState(charger.ChargerIdleState);
            return;
        }
    }

    public override void FixedUpdate() {

    }
}