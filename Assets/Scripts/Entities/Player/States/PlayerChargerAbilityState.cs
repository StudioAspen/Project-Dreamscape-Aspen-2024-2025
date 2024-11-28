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
    private float timeLimit = 5f;

    public PlayerChargerAbilityState(Player player) : base(player)
    {
        this.player = player;
    }

    private float boost = 2f;

    private float currentSpeed;
    private Vector3 currentVelocity;

    public override void Update()
    {
        player.ApplyGravity();

        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
        player.AccelerateToSpeed(player.MovementSpeed);
        player.GroundedMove();

        timer -= Time.deltaTime;

        if (timer <= 0) 
        {
            player.ChangeState(player.PlayerIdleState);
        }
    }


    public override void OnEnter()
    {
        timer = timeLimit;
        player.SetSpeedModifier(boost * player.SprintSpeedModifier);

        currentVelocity = player.GetVelocity();
    }

    public override void OnExit()
    {
        
    }

    public override void FixedUpdate()
    {
        
    } 
}

    
