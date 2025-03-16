using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FloatingPlatformRisingState : FloatingPlatformBaseState
{

    public override void OnEnter()
    { 
        
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, floatingPlatform.pos1, floatingPlatform.Speed * Time.deltaTime);
        if (transform.position == floatingPlatform.pos1)
        {
            floatingPlatform.ChangeState(floatingPlatform.FloatingPlatformIdleState);
        }
    }
}
