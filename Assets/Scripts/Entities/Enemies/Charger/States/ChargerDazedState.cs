using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerDazedState : EnemyBaseState
{
    private Charger charger;

    private float timer;

    public ChargerDazedState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        charger.TransitionToAnimation("Hit");

        charger.SetSpeedModifier(0f);

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        timer += charger.LocalDeltaTime;

        if(timer > charger.DazedDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        charger.ApplyGravity();
    }
}
