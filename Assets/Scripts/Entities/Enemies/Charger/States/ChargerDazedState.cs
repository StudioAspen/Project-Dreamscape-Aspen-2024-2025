using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerDazedState : EnemyBaseState
{
    private Charger charger;

    private float healthWhenEnterDazed;
    private float timer;

    public ChargerDazedState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        charger.DefaultTransitionToAnimation("Hit");

        charger.SetSpeedModifier(0f);

        healthWhenEnterDazed = charger.CurrentHealth;

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if(timer > charger.DazedDuration)
        {
            charger.ChangeState(charger.EnemyIdleState);
            return;
        }

        if (charger.CurrentHealth < healthWhenEnterDazed)
        {
            charger.ChangeState(charger.ChargerDamagedState);
            return;
        }
    }

    public override void FixedUpdate() { }
}