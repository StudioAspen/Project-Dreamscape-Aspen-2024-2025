using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class FloatingPlatformSinkingState : FloatingPlatformBaseState
{
    private float timer;   

    public override void OnEnter()
    {
        floatingPlatform.pos1 = floatingPlatform.Pos1.transform.position;
        floatingPlatform.pos2 = floatingPlatform.Pos2.transform.position;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, floatingPlatform.pos2, floatingPlatform.Speed * Time.deltaTime);
        if (transform.position.y == floatingPlatform.pos2.y)
        {
            timer += Time.deltaTime;
            if (timer > floatingPlatform.Pos2Dur)
            {
                floatingPlatform.ChangeState(floatingPlatform.FloatingPlatformRisingState);
            }
        }
    }
}
