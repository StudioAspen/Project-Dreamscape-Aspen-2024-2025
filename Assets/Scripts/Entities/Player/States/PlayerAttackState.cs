using System;
using System.Collections.Generic;
using System.IO;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private PlayerCombat playerCombat;

    public ComboDataSO ComboData { get; private set; }

    private float duration;
    private float timer;

    public PlayerAttackState(Player player) : base(player)
    {
        this.player = player;
        playerCombat = player.GetComponent<PlayerCombat>();
    }

    /// <summary>
    /// Sets the combo data for the player.
    /// Needs to be called everytime you change to the PlayerAttackState.
    /// </summary>
    /// <param name="comboData">The combo data to set.</param>
    public void SetCombo(ComboDataSO comboData)
    {
        ComboData = comboData;
    }

    public override void OnEnter()
    {
        playerCombat.Weapon.OnWeaponStartSwing?.Invoke(player); // invoke the weapon start swing event

        playerCombat.Weapon.ClearEnemiesHitList(); // allows all enemies to get hit again

        playerCombat.Weapon.SetPercentDamage(ComboData.PercentDamage); // set the damage percent for this combo
        playerCombat.Weapon.ConfigureImpactFrames(ComboData.ImpactFramesTimeScale, ComboData.ImpactFramesDuration); // configure the impact frames for this combo

        player.SetComboAnimationSpeed(ComboData.ComboClipAnimationSpeed); // set the animation speed for this combo

        player.TransitionToAnimation($"Combos.{ComboData.ComboClip.name}"); // play the combo animations

        playerCombat.CanCancelAnimation = false; // prevents the player from cancelling the animation
        playerCombat.IsAnimationPlaying = true; // true when combo is playing, false when done, becomes false from the animation event

        duration = ComboData.ComboClip.length / ComboData.ComboClipAnimationSpeed; // set the duration of the combo
        timer = 0f; // reset the timer

        player.ApplyRootMotion = ComboData.HasRootMotion; // apply root motion if the combo has it

        if(player.IsGrounded) player.ApplyRotationToNextMovement(); // if grounded makes the player face the direction they are facing and moving

        playerCombat.Weapon.OnWeaponHit.AddListener(PlayerCombat_OnWeaponHit); // listen for weapon hits
    }

    public override void OnExit()
    {
        playerCombat.Weapon.OnWeaponEndSwing?.Invoke(player); // invoke the weapon end swing event

        playerCombat.IsAnimationPlaying = false; // disables the animation playing bool in case the animation event doesnt do it
        player.ApplyRootMotion = false; // stops root motion
        playerCombat.CanCombo = false; // prevents the player from comboing again since they missed the window

        playerCombat.EndHit(); // stops the hitbox on the weapon

        player.InstantlySetGroundedSpeed(0f); // stops the player from moving

        playerCombat.Weapon.OnWeaponHit.RemoveListener(PlayerCombat_OnWeaponHit); // remove the onhit listener
    }

    public override void Update()
    {
        player.ApplyGravity();

        if (!playerCombat.IsAnimationPlaying) // if the animation is done playing, go back to the default state
        {
            player.ChangeState(player.DefaultState);
            return;
        }

        HandleAnimationCancellingBuffer();

        // update new target rotation player if they are moving and grounded
        if (player.IsGrounded && player.MoveDirection != Vector3.zero) player.ApplyRotationToNextMovement(); 

        player.RotateToTargetRotation();
        player.AccelerateToSpeed(0f);
        player.InstantlySetGroundedSpeed(player.GetGroundedVelocity().magnitude);
        player.GroundedMove();
    }

    public override void FixedUpdate()
    {

    }

    private void HandleAnimationCancellingBuffer()
    {
        timer += Time.deltaTime;
        if (timer > duration / 2) playerCombat.CanCancelAnimation = true;
    }

    private void PlayerCombat_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damage)
    {
        if (ComboData.WillLaunchUpwards && !victim.WillDieFromDamage(damage))
        {
            victim.ForceChangeToLaunchState(Vector3.up, ComboData.AirLaunchForce, 2f);
            if (ComboData.AirLaunchForce > 0) player.Launch(Vector3.up, ComboData.AirLaunchForce);

            player.ApplyRotationToNextMovement(player.LookAt(victim.transform.position));

            return;
        }

        if (!player.IsGrounded && !victim.IsGrounded && !victim.WillDieFromDamage(damage))
        {
            victim.ForceChangeToLaunchState(Vector3.up, ComboData.AirLaunchForce, 2f);
            if(ComboData.AirLaunchForce > 0) player.Launch(Vector3.up, ComboData.AirLaunchForce);

            player.ApplyRotationToNextMovement(player.LookAt(victim.transform.position));
        }
    }
}

