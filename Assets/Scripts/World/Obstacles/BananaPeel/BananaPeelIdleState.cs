using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BananaPeelIdleState : BananaPeelBaseState
{
    public override void OnEnter()
    {

    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnOnTriggerEnter(Collider other)
    {
        bananapeel.ChangeState(bananapeel.BananaPeelSteppedState);
    }
}
