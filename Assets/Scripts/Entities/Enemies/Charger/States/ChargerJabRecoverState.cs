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
        charger.DefaultTransitionToAnimation("RightJab");

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (timer > charger.JabRecoverDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }

        charger.DefaultTransitionToAnimation("RightJab");
    }

    public override void FixedUpdate()
    {
        
    }
}