using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamMushroomIdleState : DreamMushroomBaseState
{
    public override void OnEnter()
    {
        dreamMushroom.OnDamaged += DreamMushroom_OnDamage;
    }

    

    public override void OnExit()
    {
        dreamMushroom.OnDamaged -= DreamMushroom_OnDamage;
    }

    public override void OnUpdate()
    {

    }
        
    private void DreamMushroom_OnDamage(Obstacle damagedobstacle, Vector3 hitPoint, GameObject source)
    {
        dreamMushroom.ChangeState(dreamMushroom.DreamMushroomBurstState);


    }

    public override void OnOnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out Player player))
        {
            dreamMushroom.ChangeState(dreamMushroom.DreamMushroomBurstState);
        }
    }
}
