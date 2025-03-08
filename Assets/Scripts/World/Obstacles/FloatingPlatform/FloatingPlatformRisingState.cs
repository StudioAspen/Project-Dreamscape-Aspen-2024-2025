using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FloatingPlatformRisingState : FloatingPlatformBaseState
{
    private float speed;
    private float diry;
    public override void OnEnter()
    {
        Debug.Log("rising");   
        
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        Vector3 moveDir = new Vector3(0, diry, 0);
        transform.position += moveDir * speed * Time.deltaTime;
    }
}
