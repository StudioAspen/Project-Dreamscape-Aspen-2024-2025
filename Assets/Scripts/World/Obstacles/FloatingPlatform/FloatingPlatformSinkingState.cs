using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class FloatingPlatformSinkingState : FloatingPlatformBaseState
{
    private float speed = 0.5f;
    
    public override void OnEnter()
    {
        Debug.Log("Sinking");
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, floatingPlatform.pos2, speed * Time.deltaTime);
        Debug.Log(transform.position);
        if (transform.position.y <= floatingPlatform.pos2.y)
        {
            floatingPlatform.ChangeState(floatingPlatform.FloatingPlatformRisingState);
        }
    }
}
