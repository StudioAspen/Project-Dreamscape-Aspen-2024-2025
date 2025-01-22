using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Shield Elite Variant", menuName = "Status Effect/Enemy Variants/Elite/Shield")]
public class ShieldEliteVariantStatusEffectSO : EliteVariantStatusEffectSO
{
    [field: Header("Config")]
    [field: SerializeField] public float TodoShieldPrefab { get; private set; } = 1f;

    private protected override void OnApply()
    {
        base.OnApply();

        enemy.SetMaxHealth(0, false); // Make the enemy invincible (MaxHealth = 0 means unkillable)

        enemy.OnEntityTakeDamage += Enemy_OnEntityTakeDamage;
    }

    public override void Cancel()
    {
        base.Cancel();

        enemy.OnEntityTakeDamage -= Enemy_OnEntityTakeDamage; // Just in case the enemy dies before the shield is broken
    }

    private void Enemy_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        enemy.OnEntityTakeDamage -= Enemy_OnEntityTakeDamage; // Remove the event listener because this only happens once

        enemy.SetMaxHealthModifier(enemy.MaxHealthModifier); // Make the enemy killable again
        enemy.TakeDamage(0, enemy.GetColliderCenterPosition(), source, false); // Deal 0 damage to update the health bar
    }
}
