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

        playerCombat.Weapon.SetDamageRange(ComboData.ComboDamageRange);

        player.ReplaceComboAnimationClip(ComboData.ComboClip);
        player.SetComboAnimationSpeed(ComboData.ComboClipAnimationSpeed);
        player.TransitionToAnimation("Combo", 0.05f);

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
        playerCombat.DisableWeaponTriggers();

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

    private void PlayerCombat_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint)
    {
        if (ComboData.WillLaunchUpwards)
        {
            //Physics.gravity = -10 * Vector3.up;

            victim.TryChangeToLaunchState(Vector3.up, ComboData.AirLaunchForce, 2f);

            source.Launch(Vector3.up, ComboData.AirLaunchForce);
        }

        if (ComboData.WillIgnoreGravity)
        {
            victim.ForceChangeToLaunchState(Vector3.up, 5f, 2f);

            source.Launch(Vector3.up, 5f);
        }
    }
}

