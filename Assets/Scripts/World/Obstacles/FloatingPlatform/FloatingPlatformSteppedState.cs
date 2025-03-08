using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatformSteppedState : FloatingPlatformBaseState
{
    private float timer;

    public override void OnEnter()
    {
        timer = 0;
        Debug.Log("SteepedState");
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        timer++;  
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
