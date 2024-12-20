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

    private float extraPercentDamage = 100f;

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

    /// <summary>
    /// Sets the extra percent damage for this swing.
    /// </summary>
    /// <param name="extraPercentDamage">The extra percent damage to set.</param>
    public void SetExtraPercentDamage(float extraPercentDamage)
    {
        this.extraPercentDamage = extraPercentDamage;
    }

    public override void OnEnter()
    {
        playerCombat.Weapon.OnWeaponStartSwing?.Invoke(player); // invoke the weapon start swing event

        playerCombat.Weapon.ClearEnemiesHitList(); // allows all enemies to get hit again

        playerCombat.Weapon.SetPercentDamage(ComboData.PercentDamage * extraPercentDamage/100f); // set the damage percent for this combo
        playerCombat.Weapon.ConfigureImpactFrames(ComboData.ImpactFramesTimeScale, ComboData.ImpactFramesDuration); // configure the impact frames for this combo

        playerCombat.SetComboAnimationSpeed(ComboData.ComboClipAnimationSpeed); // set the animation speed for this combo

        player.TransitionToAnimation($"Combos.{ComboData.ComboClip.name}"); // play the combo animations

        playerCombat.CanCancelAnimation = false; // prevents the player from cancelling the animation
        playerCombat.IsAnimationPlaying = true; // true when combo is playing, false when done, becomes false from the animation event

        duration = ComboData.ComboClip.length / ComboData.ComboClipAnimationSpeed; // set the duration of the combo
        timer = 0f; // reset the timer

        player.UseRootMotion = ComboData.HasRootMotion; // apply root motion if the combo has it

        if(player.IsGrounded) player.ApplyRotationToNextMovement(); // if grounded makes the player face the direction they are facing and moving

        playerCombat.Weapon.OnWeaponHit += PlayerCombat_OnWeaponHit; // listen for weapon hits
    }

    public override void OnExit()
    {
        playerCombat.Weapon.OnWeaponEndSwing?.Invoke(player); // invoke the weapon end swing event

        playerCombat.IsAnimationPlaying = false; // disables the animation playing bool in case the animation event doesnt do it
        player.UseRootMotion = false; // stops root motion
        playerCombat.CanCombo = false; // prevents the player from comboing again since they missed the window

        playerCombat.EndHit(); // stops the hitbox on the weapon

        player.InstantlySetHorizontalSpeed(0f); // stops the player from moving

        extraPercentDamage = 100f; // reset the extra damage percentage

        playerCombat.Weapon.OnWeaponHit -= PlayerCombat_OnWeaponHit; // remove the onhit listener
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
        player.AccelerateToHorizontalSpeed(0f);
        player.InstantlySetHorizontalSpeed(player.GetHorizontalVelocity().magnitude);
        player.ApplyHorizontalVelocity();
    }

    public override void FixedUpdate()
    {

    }

    /// <summary>
    /// Prevents the player from cancelling the animation for the first half of the animation.
    /// Ensures that the player cant unexpectedly cancel the animation in a bug.
    /// </summary>
    private void HandleAnimationCancellingBuffer()
    {
        timer += player.LocalDeltaTime;
        if (timer > duration / 2) playerCombat.CanCancelAnimation = true;
    }

    private void PlayerCombat_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damage)
    {
        TryLaunchVictim(victim, damage);
        TryAirComboVictim(victim, damage);
    }

    /// <summary>
    /// Tries to launch victim on hit to start air combo
    /// </summary>
    /// <param name="victim">The victim entity.</param>
    /// <param name="damage">The damage inflicted on the victim.</param>
    private void TryLaunchVictim(Entity victim, int damage)
    {
        if (!ComboData.WillLaunchUpwards) return;
        if (victim.WillDieFromDamage(damage)) return;

        victim.ForceChangeToLaunchState(Vector3.up, ComboData.AirLaunchForce, 2f);

        if (ComboData.AirLaunchForce > 0) player.Launch(Vector3.up, ComboData.AirLaunchForce);

        player.ApplyRotationToNextMovement(player.LookAt(victim.transform.position));
    }

    /// <summary>
    /// Tries to air combo victim on hit.
    /// </summary>
    /// <param name="victim">The victim entity.</param>
    /// <param name="damage">The damage inflicted on the victim.</param>
    private void TryAirComboVictim(Entity victim, int damage)
    {
        if (player.IsGrounded) return;
        if (victim.IsGrounded) return;

        if (victim.WillDieFromDamage(damage)) victim.Launch(Vector3.up, ComboData.AirLaunchForce);
        else victim.ForceChangeToLaunchState(Vector3.up, ComboData.AirLaunchForce, 2f);

        if (ComboData.AirLaunchForce > 0) player.Launch(Vector3.up, ComboData.AirLaunchForce);
        else player.ResetYVelocity();

        player.ApplyRotationToNextMovement(player.LookAt(victim.transform.position));
    }
}

