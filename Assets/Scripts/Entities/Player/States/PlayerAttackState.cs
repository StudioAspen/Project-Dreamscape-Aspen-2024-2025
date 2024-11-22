using Animancer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private PlayerCombat playerCombat;

    public ComboDataSO ComboData { get; private set; }

    public PlayerAttackState(Player player) : base(player)
    {
        this.player = player;
        playerCombat = player.GetComponent<PlayerCombat>();
    }

    public override void OnEnter()
    {
        playerCombat.Weapon.OnWeaponStartSwing?.Invoke(player);
        playerCombat.Weapon.ClearEnemiesHitList();

        playerCombat.Weapon.SetPercentDamage(ComboData.PercentDamage);

        AnimancerState state = player.PlayAnimation(ComboData.ComboClip, 0.25f);

        state.Speed = ComboData.ComboClipAnimationSpeed;

        playerCombat.IsAnimationPlaying = true;
        player.ApplyRootMotion = ComboData.HasRootMotion;

        player.ApplyRotationToNextMovement();

        playerCombat.Weapon.OnWeaponHit.AddListener(PlayerCombat_OnWeaponHit);
    }

    public override void OnExit()
    {
        playerCombat.Weapon.OnWeaponEndSwing?.Invoke(player);
        playerCombat.IsAnimationPlaying = false;
        player.ApplyRootMotion = false;

        playerCombat.EndHit();
        playerCombat.CanCombo = false;

        player.InstantlySetGroundedSpeed(0f);

        //if (ComboData.WillIgnoreGravity) player.ResetYVelocity();

        playerCombat.Weapon.OnWeaponHit.RemoveListener(PlayerCombat_OnWeaponHit);
    }

    public override void Update()
    {
        player.ApplyGravity();

        if (!playerCombat.IsAnimationPlaying) player.ChangeState(player.DefaultState);

        if (player.MoveDirection != Vector3.zero) player.ApplyRotationToNextMovement();

        player.RotateToTargetRotation();
        player.AccelerateToSpeed(0f);
        player.InstantlySetGroundedSpeed(player.GetGroundedVelocity().magnitude);
        player.GroundedMove();

        player.RotateToTargetRotation();
    }

    public override void FixedUpdate()
    {

    }

    public void SetCombo(ComboDataSO comboData)
    {
        ComboData = comboData;
    }

    private void PlayerCombat_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damage)
    {
        if (ComboData.WillLaunchUpwards)
        {
            victim.TryChangeToLaunchState(Vector3.up, ComboData.AirLaunchForce, 2f);

            source.Launch(Vector3.up, ComboData.AirLaunchForce);

            return;
        }

        if (!player.IsGrounded)
        {
            victim.ForceChangeToLaunchState(Vector3.up, ComboData.AirLaunchForce, 2f);

            source.Launch(Vector3.up, ComboData.AirLaunchForce);
        }
    }
}

