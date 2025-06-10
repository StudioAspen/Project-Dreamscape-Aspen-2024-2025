using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatformSteppedState : FloatingPlatformBaseState
{
    private float timer;

    public override void OnEnter()
    {
        timer = 0;
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;  
        if (timer <=7)
        {
            if (timer <= 2)
            {
                //apply warning effect
            }
        }
        else
        {
            floatingPlatform.ChangeState(floatingPlatform.FloatingPlatformSinkingState);
        }
    }
}
