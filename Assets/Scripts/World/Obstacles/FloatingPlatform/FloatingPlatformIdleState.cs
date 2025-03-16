using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FloatingPlatformIdleState : FloatingPlatformBaseState
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
        floatingPlatform.ChangeState(floatingPlatform.FloatingPlatformSteppedState);
    }
}
