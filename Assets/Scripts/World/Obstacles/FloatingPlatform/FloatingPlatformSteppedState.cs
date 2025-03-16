using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatformSteppedState : FloatingPlatformBaseState
{
    private float timer;

    public override void OnEnter()
    {
        timer = 0;
        Debug.Log("SteppedState");
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        timer = timer + Time.deltaTime;  
        if (timer <=7)
        {
            if (timer <= 5)
            {
                Debug.Log("warning");
            }
        }
        else
        {
            floatingPlatform.ChangeState(floatingPlatform.FloatingPlatformSinkingState);
        }
    }
}
