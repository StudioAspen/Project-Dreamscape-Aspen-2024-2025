using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggYolkIdleState : EggYolkBaseState
{
    public override void OnEnter()
    {
        eggYolk.OnDamaged += EggYolk_OnDamaged;
    }

    public override void OnExit()
    {
        eggYolk.OnDamaged -= EggYolk_OnDamaged;
    }

    public override void OnUpdate()
    {

    }

    private void EggYolk_OnDamaged(Obstacle damagedObstacle, Vector3 hitPoint, GameObject source)
    {
        eggYolk.ChangeState(eggYolk.EggYolkOnHitState);
    }

}
