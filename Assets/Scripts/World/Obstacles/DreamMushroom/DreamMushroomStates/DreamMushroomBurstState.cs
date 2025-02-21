using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamMushroomBurstState : DreamMushroomBaseState
{
    
    public override void OnEnter()
    {
        Debug.Log("burst");
        // NAUSEOUS AND SCREEN SHAKE
        dreamMushroom.ChangeState(dreamMushroom.DreamMushroomSplotchState);

    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        
    }
}
