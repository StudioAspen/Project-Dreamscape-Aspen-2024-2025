using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    private float boost = 2.5f;

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
    }

    public override void OnExit()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    private bool IsOwnDamageableEntityCollider(Collider hit)
    {
        // check if hit is a child of player's collider
        Player selfPlayer = hit.GetComponentInParent<Player>();

        if (selfPlayer == null) return false;
        if (selfPlayer == player) return true;

        return false;
    }

    private bool DidPlayerHitEnemyEntity(Collider hit, out Entity entity)
    {
        entity = hit.GetComponentInParent<Entity>();

        if (entity == null) return false;
        if (entity.Team == player.Team) return false;

        return true;
    }

    private void TryFlingEntity(Entity entity, Vector3 direction, float force, float stunDuration)
    {
        if (entity.GetType() == typeof(Player)) return;

        entity.TryChangeToLaunchState(direction, force, stunDuration);
    }

}

    
