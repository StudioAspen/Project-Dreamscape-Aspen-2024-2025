using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerDamagedState : EnemyBaseState
{
    private Charger charger;
    private float damagedStateTimer;
    public ChargerDamagedState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        Debug.Log("entering damaged state");
        charger.InDamagedState = true;
        damagedStateTimer = 0f;
    }
    public override void OnExit()
    {}

    public override void Update()
    {
        damagedStateTimer += Time.deltaTime;
        Debug.Log(damagedStateTimer);
        Debug.Log(charger.DamagedStateDuration);
        if (damagedStateTimer > charger.DamagedStateDuration)
        {
            charger.IsDazed = false;
            charger.ChangeState(charger.ChargerIdleState);
            return;
        }
    }

    public override void FixedUpdate()
    {

    }
}
