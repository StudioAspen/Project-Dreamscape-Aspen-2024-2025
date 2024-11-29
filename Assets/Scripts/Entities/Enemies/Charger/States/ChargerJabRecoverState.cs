using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChargerJabRecoverState : EnemyBaseState
{
    private Charger charger;

    private float timer;

    public ChargerJabRecoverState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        charger.TransitionToAnimation("RightJab");

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        timer += charger.LocalDeltaTime;

        if (timer > charger.JabRecoverDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }

        charger.TransitionToAnimation("RightJab");
    }

    public override void FixedUpdate()
    {
        charger.ApplyGravity();
    }
}