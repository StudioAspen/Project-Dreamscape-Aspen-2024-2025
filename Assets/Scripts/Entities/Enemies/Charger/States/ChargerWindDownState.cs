using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerWindDownState : EnemyBaseState {
    private Charger charger;

    private float windDownTimer;

    public ChargerWindDownState(Charger enemy) : base(enemy) {
        charger = enemy;
    }

    public override void OnEnter() {
        windDownTimer = 0f;
    }

    public override void OnExit() {

    }

    public override void Update() {
        windDownTimer += Time.deltaTime;

        if (windDownTimer > charger.WindDownDuration) {
            charger.ChangeState(charger.EnemyIdleState);
            return;
        }
    }

    public override void FixedUpdate() {

    }

}