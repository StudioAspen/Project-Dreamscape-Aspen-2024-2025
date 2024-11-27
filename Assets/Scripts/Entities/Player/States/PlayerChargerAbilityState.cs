using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class PlayerChargerAbilityState : PlayerBaseState
{
    private float timer;
    private float timeLimit = 15f;

    public PlayerChargerAbilityState(Player player) : base(player)
    {
        this.player = player;
    }

    private float boost = 2f;

    private float currentSpeed;
    private Vector3 currentVelocity;

    public override void Update()
    {
        player.SetVelocity(currentSpeed * boost * Vector3.forward);
        timer -= Time.deltaTime;

        if (timer <= 0) 
        {
            OnExit();
        }
    }


    public override void OnEnter()
    {
        timer = timeLimit;
        currentVelocity = player.GetVelocity();
        currentSpeed = player.GetVelocity().magnitude;
    }

    public override void OnExit()
    {
        player.SetVelocity(currentVelocity);
        player.ChangeState(player.PlayerIdleState);
    }

    public override void FixedUpdate()
    {
        
    }

    
}

    
