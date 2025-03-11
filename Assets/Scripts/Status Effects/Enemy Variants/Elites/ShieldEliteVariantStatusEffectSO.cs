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

        enemy.SetInvincible(true);

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

        enemy.SetInvincible(false);

        shieldVFXInstance.PlayEndAnimation(() => Destroy(shieldVFXInstance.gameObject));
    }
}
