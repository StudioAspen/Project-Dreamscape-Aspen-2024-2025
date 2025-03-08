using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class FloatingPlatformSinkingState : FloatingPlatformBaseState
{
    private float speed;
    private float sinkTime = 10f;
    private float Timer;
    private float diry;

    public override void OnEnter()
    {
        Debug.Log("Sinking");
        Timer = 0;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        Vector3 moveDir = new Vector3(0, diry, 0);
        transform.position -= moveDir * speed * Time.deltaTime;
        Timer = Timer + Time.deltaTime;
        if(Timer > sinkTime)
        {
            floatingPlatform.ChangeState(floatingPlatform.FloatingPlatformRisingState);
        }
    }
}
