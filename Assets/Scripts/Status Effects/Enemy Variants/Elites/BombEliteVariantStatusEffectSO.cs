using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bomb Elite Variant", menuName = "Status Effect/Enemy Variants/Elite/Bomb")]
public class BombEliteVariantStatusEffectSO : EliteVariantStatusEffectSO
{
    [field: Header("Config")]
    [field: SerializeField] public float ExplosionDelay { get; private set; } = 1f;
    [field: SerializeField] public float ExplosionRadius { get; private set; } = 5f;
    [field: SerializeField] public float ExplosionDamageMultiplier { get; private set; } = 1.5f;
    [field: SerializeField] public float ExplosionLaunchForce { get; private set; } = 15f;
    [field: SerializeField] public float ExplosionStunDuration { get; private set; } = 2f;

    [field: Header("Camera Shake")]
    [field: SerializeField] public float CameraShakeDuration { get; private set; } = 1f;
    [field: SerializeField] public float CameraShakeStrength { get; private set; } = 10f;

    private protected override void OnApply()
    {
        base.OnApply();

        enemy.OnEntityDeath += Enemy_OnEntityDeath;
    }

    public override void Cancel()
    {
        base.Cancel();

        enemy.OnEntityDeath -= Enemy_OnEntityDeath;
    }

    private void Enemy_OnEntityDeath(GameObject killer)
    {
        Vector3 explosionCenter = enemy.transform.position;

        DOVirtual.DelayedCall(ExplosionDelay, () => Explode(explosionCenter), false);
    }

    private void Explode(Vector3 center)
    {
        Entity.DamageEnemyEntitiesWithAOELaunch(enemy, center, ExplosionRadius, ExplosionDamageMultiplier, ExplosionLaunchForce, ExplosionStunDuration);
        CameraShakeManager.Instance.ShakeCamera(CameraShakeStrength, 1f, CameraShakeDuration);
        CustomDebug.InstantiateTemporarySphere(center, ExplosionRadius, 1f, new Color(1f, 0, 0, 0.2f));
    }
}