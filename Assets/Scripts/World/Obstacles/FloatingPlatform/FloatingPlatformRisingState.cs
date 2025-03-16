using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FloatingPlatformRisingState : FloatingPlatformBaseState
{
    private float speed = 0.2f;

    public override void OnEnter()
    {
        Debug.Log("rising");   
        
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, floatingPlatform.pos1, speed * Time.deltaTime);
        if (transform.position == floatingPlatform.pos1)
        {
            floatingPlatform.ChangeState(floatingPlatform.FloatingPlatformIdleState);
        }
    }
}
