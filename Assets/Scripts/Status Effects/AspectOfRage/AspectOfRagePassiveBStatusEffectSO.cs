using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Rage/Passive B")]
public class AspectOfRagePassiveBStatusEffectSO : StatusEffectSO
{
    private PlayerCombat playerCombat;
    private Player player;

    [field: Header("Aspect of Rage Passive B: Settings")]
    [field: SerializeField] public ChargeAttackActivatedStatusEffectSO ChargedAttackActivatedStatusEffect { get; private set; }
    [field: SerializeField] public float MaxChargeDuration { get; private set; } = 5f;
    [field: SerializeField] public List<float> ChargeDurationBonusDamageMultipliers { get; private set; } = new List<float>();
    [field: SerializeField] public float ChargedOnHitExplosionDamageMultiplier { get; private set; } = 1f;
    [field: SerializeField] public float ChargedOnHitExplosionRadius { get; private set; } = 5f;

    [field: Header("Aspect of Rage Passive B: Perfect Timing Settings")]
    [field: SerializeField] public float PerfectTimingWindowDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float PerfectTimingBonusDamageMultiplier { get; private set; } = 2f;
    [field: SerializeField] public float PerfectTimingOnHitExplosionRadiusMultiplier { get; private set; } = 2f;
    private bool isCurrentSwingPerfectlyTimed = false;

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        playerCombat = entity.GetComponent<PlayerCombat>();
        if (playerCombat == null)
        {
            Debug.LogError($"{name}: PlayerCombat not found on player: {entity.name}");
            RemoveSelf(); // If theres no PlayerCombat, remove this passive
            return;
        }

        player = playerCombat.GetComponent<Player>();

        // Enable charging for the player
        ChargeAttackActivatedStatusEffectSO chargeAttackActivatedStatusEffectInstance =
            entityStatusEffectorOwner.ApplyStatusEffect(ChargedAttackActivatedStatusEffect, entity.gameObject) as ChargeAttackActivatedStatusEffectSO;
        chargeAttackActivatedStatusEffectInstance.SetMaxChargeDuration(MaxChargeDuration); // Set the new max charge duration
        player.PlayerChargeState.SetChargeStatusEffect(chargeAttackActivatedStatusEffectInstance);

        playerCombat.OnChargeRelease += PlayerCombat_OnChargeRelease;
        playerCombat.Weapon.OnWeaponEndSwing += Weapon_OnWeaponEndSwing;
        playerCombat.Weapon.OnWeaponHit += Weapon_OnWeaponHit;
    }

    public override void Cancel()
    {
        base.Cancel();

        entityStatusEffectorOwner.RemoveStatusEffect(ChargedAttackActivatedStatusEffect.GetType(), true);

        playerCombat.OnChargeRelease -= PlayerCombat_OnChargeRelease;
        playerCombat.Weapon.OnWeaponEndSwing -= Weapon_OnWeaponEndSwing;
        playerCombat.Weapon.OnWeaponHit -= Weapon_OnWeaponHit;
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        AspectOfRagePassiveBStatusEffectSO overridingStatusEffect = newStatusEffect as AspectOfRagePassiveBStatusEffectSO;

        // Set new max charge duration
        entityStatusEffectorOwner.TryGetStatusEffect<ChargeAttackActivatedStatusEffectSO>()
            .SetMaxChargeDuration(overridingStatusEffect.MaxChargeDuration);

        // Set new damage bonus values
        ChargeDurationBonusDamageMultipliers = overridingStatusEffect.ChargeDurationBonusDamageMultipliers;

        // Set new charged on hit config values
        ChargedOnHitExplosionDamageMultiplier = overridingStatusEffect.ChargedOnHitExplosionDamageMultiplier;
        ChargedOnHitExplosionRadius = overridingStatusEffect.ChargedOnHitExplosionRadius;

        // Set extended passive config values
        PerfectTimingWindowDuration = overridingStatusEffect.PerfectTimingWindowDuration;
        PerfectTimingBonusDamageMultiplier = overridingStatusEffect.PerfectTimingBonusDamageMultiplier;
        PerfectTimingOnHitExplosionRadiusMultiplier = overridingStatusEffect.PerfectTimingOnHitExplosionRadiusMultiplier;
    }

    public override void Update()
    {
        base.Update();

        if(IsPerfectTimingBonusActive(player.PlayerChargeState.Timer))
            Debug.Log($"Perfect Timing Window!");
    }

    /// <summary>
    /// Checks if the perfect timing bonus is active based on the charge duration.
    /// </summary>
    /// <param name="chargeDuration">The duration of the charge attack.</param>
    /// <returns>True if the perfect timing bonus is active, false otherwise.</returns>
    private bool IsPerfectTimingBonusActive(float chargeDuration)
    {
        if (PerfectTimingWindowDuration <= 0) return false;

        return chargeDuration >= MaxChargeDuration
            && chargeDuration <= MaxChargeDuration + PerfectTimingWindowDuration;
    }

    private void PlayerCombat_OnChargeRelease(int attackInputNumber, float chargeDuration)
    {
        float bonusDamageFromDuration = CalculateBonusOnHitDamageMultiplierFromChargeDuration(chargeDuration);
        float bonusDamageFromPerfectTiming = IsPerfectTimingBonusActive(chargeDuration) ? PerfectTimingBonusDamageMultiplier : 1f;
        isCurrentSwingPerfectlyTimed = IsPerfectTimingBonusActive(chargeDuration);

        Debug.Log($"Duration Bonus: {bonusDamageFromDuration}, Timing Bonus: {bonusDamageFromPerfectTiming}, Total: {bonusDamageFromDuration * bonusDamageFromPerfectTiming}");
        player.PlayerAttackState.SetBonusDamageMultiplier(bonusDamageFromDuration * bonusDamageFromPerfectTiming);
    }

    private void Weapon_OnWeaponEndSwing(Entity attacker, ComboDataSO combo)
    {
        isCurrentSwingPerfectlyTimed = false;
    }

    private void Weapon_OnWeaponHit(Entity attacker, Entity victim, Vector3 hitPoint, int damage)
    {
        bool isChargedHit = player.PlayerAttackState.ComboData.ComboInputs.Contains(ComboAction.CHARGED_ATTACK1)
            || player.PlayerAttackState.ComboData.ComboInputs.Contains(ComboAction.CHARGED_ATTACK2);

        if(!isChargedHit) return;

        float explosionRadius = ChargedOnHitExplosionRadius *
            (isCurrentSwingPerfectlyTimed ? PerfectTimingOnHitExplosionRadiusMultiplier : 1f);

        ExplodeOnHit(attacker, victim, hitPoint, explosionRadius);
    }

    /// <summary>
    /// Calculates the bonus damage based on the charge duration.
    /// </summary>
    /// <param name="chargeDuration">The duration of the charge attack.</param>
    /// <returns>The bonus damage based on the charge duration.</returns>
    private float CalculateBonusOnHitDamageMultiplierFromChargeDuration(float chargeDuration)
    {
        if (ChargeDurationBonusDamageMultipliers.Count == 0) return 1f;

        float intervalSize = MaxChargeDuration / ChargeDurationBonusDamageMultipliers.Count;

        for (int i = 0; i < ChargeDurationBonusDamageMultipliers.Count; i++)
        {
            if (chargeDuration <= intervalSize)
            {
                return 1f;
            }
            if (chargeDuration <= intervalSize * (i + 2))
            {
                return ChargeDurationBonusDamageMultipliers[i];
            }
        }

        return ChargeDurationBonusDamageMultipliers[ChargeDurationBonusDamageMultipliers.Count - 1];
    }

    /// <summary>
    /// Explodes on hit, dealing damage to nearby enemy entities.
    /// </summary>
    /// <param name="attacker">The entity that initiated the attack.</param>
    /// <param name="victim">The entity that was hit.</param>
    /// <param name="hitPoint">The point of impact.</param>
    private void ExplodeOnHit(Entity attacker, Entity victim, Vector3 hitPoint, float radius)
    {
        // make a list and grab all non-dead entities nearby
        List<Entity> enemyList = Entity.GetEntitiesThroughAOE(hitPoint, radius, false);
        for (int i = 0; i < enemyList.Count; i++) // loop through all entities and deal damage to only the victim's allies
        {
            Entity enemy = enemyList[i]; // current entity in the loop

            if (enemy == victim) continue; // filter out victim
            if (enemy.Team != victim.Team) continue; // filter out attacker's allies

            int explosionDamage = attacker.CalculateDamage(ChargedOnHitExplosionDamageMultiplier); // calculate the damage

            attacker.DealDamageToOtherEntity(enemy, explosionDamage, hitPoint); // deal damage to enemy entities
        }

        CustomDebug.InstantiateTemporarySphere(hitPoint, radius, 0.25f, new Color(1f, 0, 0, 0.2f));
    }
}