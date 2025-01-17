using System;
using System.Collections.Generic;
using System.IO;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private PlayerCombat playerCombat;

    [field: Header("Config")]
    [field: SerializeField] public float AttackNearbyRadius { get; private set; } = 5f;
    [field: SerializeField] public float AttackNearbyInDirectionHalfAngle { get; private set; } = 25f;

    public ComboDataSO ComboData { get; private set; }

    private float extraPercentDamage = 100f;

    private float duration;
    private float timer;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);
        playerCombat = player.GetComponent<PlayerCombat>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackNearbyRadius);

        if (player == null) return;
        if (!player.IsMoving) return;

        Gizmos.DrawLine(transform.position, transform.position + player.TargetForwardDirection * AttackNearbyRadius);

        Vector3 leftPoint = Quaternion.Euler(0f, -AttackNearbyInDirectionHalfAngle, 0f) * player.TargetForwardDirection;
        Vector3 rightPoint = Quaternion.Euler(0f, AttackNearbyInDirectionHalfAngle, 0f) * player.TargetForwardDirection;

        Gizmos.DrawLine(transform.position, transform.position + leftPoint * AttackNearbyRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightPoint * AttackNearbyRadius);
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
    public void SetBonusPercentDamage(float extraPercentDamage)
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

        //player.ReplaceOneShotAnimationClip(ComboData.ComboClip, "AbilityPlaceholder");
        //player.TransitionToAnimation("Ability");
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

    public override void OnUpdate()
    {
        player.ApplyGravity();

        if (!playerCombat.IsAnimationPlaying) // if the animation is done playing, go back to the default state
        {
            player.ChangeState(player.DefaultState);
            return;
        }

        HandleAnimationCancellingBuffer();

        TryLookAtClosestTarget();

        player.AccelerateToHorizontalSpeed(0f);
        player.InstantlySetHorizontalSpeed(player.GetHorizontalVelocity().magnitude);
        player.ApplyHorizontalVelocity();
    }

    /// <summary>
    /// Tries to look at the closest target for the player to attack.
    /// If there is no nearby target, the player will look at the direction they are moving.
    /// If there is a nearby target, the player will look at the target.
    /// </summary>
    private void TryLookAtClosestTarget()
    {
        // only update new target rotation player if they are grounded
        if (!player.IsGrounded) return;

        List<Entity> nearbyTargets = player.GetNearbyHostileEntities(AttackNearbyRadius, false);

        if (nearbyTargets.Count == 0)
        {
            // If there is no nearby target, look at the direction the player is moving
            if (player.IsMoving)
            {
                player.ApplyRotationToNextMovement();
                player.RotateToTargetRotation();
                //Debug.Log("No nearby targets, looking at direction of movement");
            }
            else
            {
                // If not moving just rotate to the direction of the camera calculated on enter this state
                player.RotateToTargetRotation();
                //Debug.Log("No nearby targets, looking at direction of camera");
            }
        }
        else
        {
            if (player.IsMoving)
            {
                // If there is a nearby target and you are pressing inputs, look at the nearest target at the direction you are trying to move
                player.ApplyRotationToNextMovement(); // Calculate the target rotation/forward based on the input and camera

                Entity closestEntity = GetClosestEntityFromPieCutout(nearbyTargets, player.TargetForwardDirection, AttackNearbyInDirectionHalfAngle);
                if (closestEntity != null)
                {
                    player.LookAt(closestEntity.transform.position);
                    //Debug.Log("Looking at closest target in direction of movement");
                }
                else
                {
                    player.RotateToTargetRotation();
                    //Debug.Log("No nearby targets in direction of movement, looking at direction of movement");
                } 
            }
            else
            {
                // If there is a nearby target and you are NOT pressing inputs, try look at the nearest target in the direction you are facing
                Entity closestEntity = GetClosestEntityFromPieCutout(nearbyTargets, player.transform.forward, AttackNearbyInDirectionHalfAngle);
                if (closestEntity != null)
                {
                    player.LookAt(closestEntity.transform.position);
                    //Debug.Log("Looking at closest target in direction of facing");
                }
                else
                {
                    player.LookAt(nearbyTargets[0].transform.position);
                    //Debug.Log("No nearby targets in direction of facing, looking at closest target");
                } 
            }
        }
    }

    /// <summary>
    /// Gets the closest entity from a pie cutout defined by a forward direction and a half angle.
    /// </summary>
    /// <param name="entities">The list of entities to search from.</param>
    /// <param name="forwardDirection">The forward direction of the pie cutout.</param>
    /// <param name="halfAngle">The half angle of the pie cutout.</param>
    /// <returns>The closest entity within the pie cutout, or null if no entities are found.</returns>
    private Entity GetClosestEntityFromPieCutout(List<Entity> entities, Vector3 forwardDirection, float halfAngle)
    {
        if (entities.Count == 0) return null;

        List<Entity> entitiesInPieCutout = new List<Entity>();

        foreach (Entity entity in new List<Entity>(entities))
        {
            Vector3 flattenedForwardDirection = new Vector3(forwardDirection.x, 0f, forwardDirection.z);
            Vector3 flattenedVectorToEntity = entity.transform.position - player.transform.position;
            flattenedVectorToEntity.y = 0f;

            float angle = Vector3.Angle(flattenedForwardDirection, flattenedVectorToEntity);

            if(angle < halfAngle) entitiesInPieCutout.Add(entity);
        }

        if (entitiesInPieCutout.Count == 0) return null;

        return entitiesInPieCutout[0];
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

    /// <summary>
    /// Checks if the player can perform a basic attack.
    /// </summary>
    /// <returns>True if the player can basic attack, false otherwise.</returns>
    public bool CanBasicAttack()
    {
        if (player.CurrentState == player.PlayerChargeState) return false;
        if (player.CurrentState == player.EntityLaunchState) return false;
        if (player.CurrentState == player.PlayerAttackState && !playerCombat.CanCombo) return false;

        return true;
    }
}

