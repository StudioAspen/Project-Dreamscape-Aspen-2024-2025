using System.Collections;
using System.Collections.Generic;
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

    public override void OnOnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out Player player))
        {
            floatingPlatform.ChangeState(floatingPlatform.FloatingPlatformSteppedState);
        }
    }
}
