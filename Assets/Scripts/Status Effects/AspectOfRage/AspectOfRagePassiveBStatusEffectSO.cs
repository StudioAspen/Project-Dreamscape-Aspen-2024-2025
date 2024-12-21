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
    [field: SerializeField] public List<float> ChargeDurationBonusPercentDamages { get; private set; } = new List<float>();
    [field: SerializeField] public float ChargedOnHitExplosionPercentDamage { get; private set; } = 100f;
    [field: SerializeField] public float ChargedOnHitExplosionRadius { get; private set; } = 5f;

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
            entityStatusEffectorOwner.RemoveStatusEffect(GetType(), false); // If theres no PlayerCombat, remove this passive
            return;
        }

        player = playerCombat.GetComponent<Player>();

        // Enable charging for the player
        ChargeAttackActivatedStatusEffectSO chargeAttackActivatedStatusEffectInstance =
            entityStatusEffectorOwner.ApplyStatusEffect(ChargedAttackActivatedStatusEffect, entity.gameObject) as ChargeAttackActivatedStatusEffectSO;
        chargeAttackActivatedStatusEffectInstance.SetMaxChargeDuration(MaxChargeDuration); // Set the new max charge duration
            
        playerCombat.OnChargeRelease += PlayerCombat_OnChargeRelease;
        playerCombat.Weapon.OnWeaponHit += Weapon_OnWeaponHit;
    }

    public override void Cancel()
    {
        base.Cancel();

        entityStatusEffectorOwner.RemoveStatusEffect(ChargedAttackActivatedStatusEffect.GetType(), true);

        playerCombat.OnChargeRelease -= PlayerCombat_OnChargeRelease;
        playerCombat.Weapon.OnWeaponHit -= Weapon_OnWeaponHit;
    }

    public override bool OnStack(StatusEffectSO newStatusEffect)
    {
        if (!base.OnStack(newStatusEffect)) return false;

        // Set new max charge duration
        EntityStatusEffector.TryGetStatusEffect<ChargeAttackActivatedStatusEffectSO>(entity.gameObject)
            .SetMaxChargeDuration((newStatusEffect as ChargeAttackActivatedStatusEffectSO).MaxChargeDuration);

        // Set new damage bonus values
        ChargeDurationBonusPercentDamages = (newStatusEffect as AspectOfRagePassiveBStatusEffectSO).ChargeDurationBonusPercentDamages;

        // Set new charged on hit config values
        ChargedOnHitExplosionPercentDamage = (newStatusEffect as AspectOfRagePassiveBStatusEffectSO).ChargedOnHitExplosionPercentDamage;
        ChargedOnHitExplosionRadius = (newStatusEffect as AspectOfRagePassiveBStatusEffectSO).ChargedOnHitExplosionRadius;

        return true;
    }

    private void PlayerCombat_OnChargeRelease(int attackInputNumber, float chargeDuration)
    {
        if(ChargeDurationBonusPercentDamages.Count == 0) return;

        float intervalSize = MaxChargeDuration / ChargeDurationBonusPercentDamages.Count;

        for(int i = 0; i < ChargeDurationBonusPercentDamages.Count; i++)
        {
            if(chargeDuration <= intervalSize)
            {
                return;
            }
            if(chargeDuration <= intervalSize * (i + 2))
            {
                player.PlayerAttackState.SetExtraPercentDamage(ChargeDurationBonusPercentDamages[i]);
                //Debug.Log($"Charge Duration: {chargeDuration}, {chargeDuration/ChargeDurationBonusPercentDamages.Count}, Bonus: {ChargeDurationBonusPercentDamages[i]}");
                return;
            }
        }
    }

    private void Weapon_OnWeaponHit(Entity attacker, Entity victim, Vector3 hitPoint, int damage)
    {
        bool isChargedHit = player.PlayerAttackState.ComboData.ComboInputs.Contains(ComboAction.CHARGED_ATTACK1)
            || player.PlayerAttackState.ComboData.ComboInputs.Contains(ComboAction.CHARGED_ATTACK2);

        if(!isChargedHit) return;

        // make a list and grab all non-dead entities nearby
        List<Entity> enemyList = Entity.GetEntitiesThroughAOE(hitPoint, ChargedOnHitExplosionRadius, false);
        for (int i = 0; i < enemyList.Count; i++) // loop through all entities and deal damage to only the victim's allies
        {
            Entity enemy = enemyList[i]; // current entity in the loop

            if (enemy == victim) continue; // filter out victim
            if (enemy.Team != victim.Team) continue; // filter out attacker's allies

            int explosionDamage = attacker.CalculateDamage(ChargedOnHitExplosionPercentDamage); // calculate the damage

            enemy.TakeDamage(explosionDamage, hitPoint, attacker.gameObject); // deal damage to enemy entities
        }

        CustomGizmos.InstantiateTemporarySphere(hitPoint, ChargedOnHitExplosionRadius, 0.25f, new Color(1f, 0, 0, 0.2f));
    }
}