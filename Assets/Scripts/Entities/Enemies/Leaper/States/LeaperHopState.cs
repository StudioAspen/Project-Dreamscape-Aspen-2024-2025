using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaperHopState : EnemyBaseState
{
    private Leaper leaper;

    public LeaperHopState(Leaper enemy) : base(enemy)
    {
        leaper = enemy;
    }

    public override void OnEnter()
    {
        leaper.DefaultTransitionToAnimation("Hop");

        leaper.Jump();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
        //enemy jumps backwards twice
        //
    }
}

