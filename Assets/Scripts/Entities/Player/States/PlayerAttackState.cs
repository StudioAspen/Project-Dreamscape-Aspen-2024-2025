using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Dreamscape.Abilities;

[System.Serializable]
public class PlayerAttackState : PlayerBaseState
{
    private PlayerCombat playerCombat;

    [field: SerializeField] public float AttackNearbyRadius { get; private set; } = 5f;
    [field: SerializeField] public float AttackNearbyInFrontHalfAngle { get; private set; } = 25f;

    public ComboDataSO ComboData { get; private set; }

    private float extraDamageMultiplier = 1f;

    private float duration;
    private float timer;

    private Coroutine weaponScaleCoroutine;

    public override void Init(Entity entity)
    {
        base.Init(entity);
        playerCombat = player.GetComponent<PlayerCombat>();
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(entity.transform.position, AttackNearbyRadius);

        if (player == null) return;
        if (!player.IsMoving) return;

        Gizmos.DrawLine(entity.transform.position, entity.transform.position + player.TargetForwardDirection * AttackNearbyRadius);

        Vector3 leftPoint = Quaternion.Euler(0f, -AttackNearbyInFrontHalfAngle, 0f) * player.TargetForwardDirection;
        Vector3 rightPoint = Quaternion.Euler(0f, AttackNearbyInFrontHalfAngle, 0f) * player.TargetForwardDirection;

        Gizmos.DrawLine(entity.transform.position, entity.transform.position + leftPoint * AttackNearbyRadius);
        Gizmos.DrawLine(entity.transform.position, entity.transform.position + rightPoint * AttackNearbyRadius);
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
    /// Sets the extra damage multiplier for this swing.
    /// </summary>
    /// <param name="extraDamageMultiplier">The extra damage multiplier to set.</param>
    public void SetBonusDamageMultiplier(float extraDamageMultiplier)
    {
        this.extraDamageMultiplier = extraDamageMultiplier;
    }

    public override void OnEnter()
    {
        playerCombat.Weapon.OnWeaponStartSwing?.Invoke(player); // invoke the weapon start swing event

        playerCombat.Weapon.ClearObjectHitList(); // allows all enemies to get hit again

        playerCombat.Weapon.SetDamageMultiplier(ComboData.DamageMultiplier * extraDamageMultiplier); // set the damage mult for this combo
        playerCombat.Weapon.ConfigureImpactFrames(ComboData.ImpactFramesTimeScale, ComboData.ImpactFramesDuration); // configure the impact frames for this combo

        duration = ComboData.ComboClip.length / ComboData.ComboClipAnimationSpeed;
        timer = 0f; // reset the timer

        player.PlayOneShotAnimation(ComboData.ComboClip, duration); // play the combo animation

        player.UseRootMotion = ComboData.HasRootMotion; // apply root motion if the combo has it

        if(player.IsGrounded) player.ApplyRotationToNextMovement(); // if grounded makes the player face the direction they are facing and moving

        playerCombat.Weapon.OnWeaponHit += PlayerCombat_OnWeaponHit; // listen for weapon hits

        if (weaponScaleCoroutine != null) playerCombat.Weapon.StopCoroutine(weaponScaleCoroutine);
        playerCombat.Weapon.StartCoroutine(StartWeaponScaleCoroutine(ComboData.WeaponScale, ComboData.WeaponScalingDuration)); // scale the weapon
    }

    public override void OnExit()
    {
        playerCombat.Weapon.OnWeaponEndSwing?.Invoke(player); // invoke the weapon end swing event

        player.UseRootMotion = false; // stops root motion
        playerCombat.CanCombo = false; // prevents the player from comboing again since they missed the window

        playerCombat.EndHit(); // stops the hitbox on the weapon

        playerCombat.StartDelayedComboListsReset(playerCombat.AttackComboResetDelay);

        player.InstantlySetHorizontalSpeed(0f); // stops the player from moving

        extraDamageMultiplier = 1f; // reset the extra damage multiplier

        playerCombat.Weapon.OnWeaponHit -= PlayerCombat_OnWeaponHit; // remove the onhit listener

        if (weaponScaleCoroutine != null) playerCombat.Weapon.StopCoroutine(weaponScaleCoroutine);
        playerCombat.Weapon.StartCoroutine(StartWeaponScaleCoroutine(1f, ComboData.WeaponScalingDuration)); // scale the weapon back
    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        timer += player.LocalDeltaTime;

        if (timer > duration) // if the animation is done playing, go back to the default state
        {
            player.ChangeState(player.DefaultState);
            return;
        }

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
            if(player.IsMoving) player.ApplyRotationToNextMovement(); // If moving calculate the target rotation/forward based on the input and camera
            player.RotateToTargetRotation();
        }
        else
        {
            player.ApplyRotationToNextMovement(); // Calculate the target rotation/forward based on the input and camera

            // If there is a nearby target, look at the target based off your camera/moving direction
            Entity closestEntityInFront = GetClosestEntityFromPieCutout(nearbyTargets, player.TargetForwardDirection, AttackNearbyInFrontHalfAngle);
            if(closestEntityInFront != null)
            {
                player.LookAt(closestEntityInFront.transform.position);
            }
            else
            {
                if(player.IsMoving) player.RotateToTargetRotation(); // If no target is in front of you, look at the direction you are moving
                else player.LookAt(nearbyTargets[0].transform.position); // If no target is in front of you, look at the closest target
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

    private void PlayerCombat_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damage)
    {
        if (ComboData.WillStun) victim.EntityStunnedState.StunEntity(player, ComboData.StunDuration);

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

        victim.ForceChangeToLaunchState(player, Vector3.up, ComboData.AirLaunchForce, 2f);
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
        else victim.ForceChangeToLaunchState(player, Vector3.up, ComboData.AirLaunchForce, 2f);

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
        if (player.CurrentState == player.EntityStunnedState) return false;
        if (player.CurrentState == player.EntityStaggeredState) return false;
        if (player.CurrentState == player.PlayerAttackState && !playerCombat.CanCombo) return false;

        return true;
    }

    private IEnumerator StartWeaponScaleCoroutine(float targetScale, float duration)
    {
        Ease easeType = Ease.OutQuint;

        float startScale = playerCombat.Weapon.transform.localScale.x;
        float endScale = targetScale * playerCombat.Weapon.OriginalScale;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float currentScale = DOVirtual.EasedValue(startScale, endScale, elapsedTime / duration, easeType);
            playerCombat.Weapon.transform.localScale = currentScale * Vector3.one;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerCombat.Weapon.transform.localScale = endScale * Vector3.one;
    }

    /// <summary>
    /// Called from playerCombat's FireAbility() method. That method is called from an animation event.
    /// </summary>
    public void FireAbility()
    {
        AbilityComboDataSO abilityComboData = ComboData as AbilityComboDataSO;
        if (abilityComboData == null) return;

        CastedAbility spawnedAbility = ObjectPoolerManager.Instance.SpawnPooledObject<CastedAbility>(abilityComboData.AbilityPrefab.gameObject);
        spawnedAbility.Init(player);
    }
}

