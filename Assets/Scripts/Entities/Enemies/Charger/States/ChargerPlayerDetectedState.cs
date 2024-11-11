using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerPlayerDetectedState : EnemyBaseState
{
    private Charger charger;
    private Coroutine playerDetectedCoroutine;

    private float healthWhenEnterDazed;


    public ChargerPlayerDetectedState(Charger enemy) : base(enemy) {
        charger = enemy;
    }

    public override void FixedUpdate() {
        
    }

    public override void OnEnter() {
        charger.SetSpeedModifier(0f);
        playerDetectedCoroutine = charger.StartCoroutine(PlayerDetectedCoroutine());
        healthWhenEnterDazed = charger.CurrentHealth;
    }

    public override void OnExit() {
        if (playerDetectedCoroutine != null) {
            charger.StopCoroutine(playerDetectedCoroutine);
            playerDetectedCoroutine = null;
        }
    }

    public override void Update() {
        if (charger.Target != null) {
            charger.LookAt(charger.Target.transform.position);
        }

        if (charger.CurrentHealth < healthWhenEnterDazed) { 
            charger.ChangeState(charger.ChargerDamagedState);
        }

    }

    private IEnumerator PlayerDetectedCoroutine() {
        yield return new WaitForSeconds(charger.PlayerDetectedDuration);
        charger.ChangeState(charger.ChargerChaseState);
    }

}
