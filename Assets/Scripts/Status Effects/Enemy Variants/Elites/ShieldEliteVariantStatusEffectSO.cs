using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Shield Elite Variant", menuName = "Status Effect/Enemy Variants/Elite/Shield")]
public class ShieldEliteVariantStatusEffectSO : EliteVariantStatusEffectSO
{
    [field: Header("Config")]
    [field: SerializeField] public ShieldVFX ShieldVFXPrefab { get; private set; }
    private ShieldVFX shieldVFXInstance;

    private protected override void OnApply()
    {
        base.OnApply();

        enemy.MaxHealth.AddMultiplier(0); // Make the enemy invincible (MaxHealth = 0 means unkillable)
        enemy.HealToFull(false); // Update current health to 0

        enemy.OnEntityTakeDamage += Enemy_OnEntityTakeDamage;

        shieldVFXInstance = Instantiate(ShieldVFXPrefab, enemy.GetColliderCenterPosition(), Quaternion.identity);
        shieldVFXInstance.Init(enemy.GetColliderLargestSize() / 2, enemy.transform, enemy.GetColliderCenterPosition() - enemy.transform.position);
        shieldVFXInstance.PlayStartAnimation();
    }

    public override void Cancel()
    {
        base.Cancel();

        enemy.OnEntityTakeDamage -= Enemy_OnEntityTakeDamage; // Just in case the enemy dies before the shield is broken

        // Just in case the enemy dies before the shield is broken
        if (shieldVFXInstance != null) shieldVFXInstance.PlayEndAnimation(() => Destroy(shieldVFXInstance.gameObject));
    }

    public override void Update()
    {
        base.Update();
    }

    private void Enemy_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        enemy.OnEntityTakeDamage -= Enemy_OnEntityTakeDamage; // Remove the event listener because this only happens once

        enemy.MaxHealth.RemoveMultiplier(0); // Make the enemy killable again
        enemy.HealToFull(false); // Update current health to full

        enemy.TakeDamage(0, Vector3.zero, source, false); // Deal 0 damage to update the health bar

        shieldVFXInstance.PlayEndAnimation(() => Destroy(shieldVFXInstance.gameObject));
    }
}
