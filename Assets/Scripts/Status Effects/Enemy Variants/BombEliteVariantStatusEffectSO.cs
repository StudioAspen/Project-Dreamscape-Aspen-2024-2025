using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bomb Elite Variant", menuName = "Status Effect/Enemy Variants/Elite/Bomb")]
public class BombEliteVariantStatusEffectSO : StatusEffectSO
{
    private Enemy enemy;
    private EntityTinter entityTinter;

    [field: Header("Shader")]
    [field: SerializeField] public Material EliteMaterial { get; private set; }

    [field: Header("Config")]
    [field: SerializeField] public float ExplosionDelay { get; private set; } = 1f;
    [field: SerializeField] public float ExplosionRadius { get; private set; } = 5f;
    [field: SerializeField] public float ExplosionPercentDamage { get; private set; } = 150f;
    [field: SerializeField] public float ExplosionLaunchForce { get; private set; } = 15f;
    [field: SerializeField] public float ExplosionStunDuration { get; private set; } = 2f;

    private void OnValidate()
    {
        Stackable = false; // force unstackable
    }

    private protected override void OnApply()
    {
        base.OnApply();

        enemy = entity as Enemy;
        if(enemy == null)
        {
            Debug.LogError($"{GetType()} can only be applied to an Enemy entity.");
            entityStatusEffectorOwner.RemoveStatusEffect(GetType(), true);
            return;
        }

        entityTinter = enemy.GetComponent<EntityTinter>();
        if(entityTinter != null)
        {
            entityTinter.RemoveAllMaterials();
            entityTinter.AddMaterial(EliteMaterial);
        }
            

        enemy.OnEntityDeath += Enemy_OnEntityDeath;
    }

    public override void Cancel()
    {
        base.Cancel();

        if (entityTinter != null) entityTinter.RestoreOriginalMaterials();

        enemy.OnEntityDeath -= Enemy_OnEntityDeath;
    }

    private void Enemy_OnEntityDeath(GameObject killer)
    {
        enemy.StartCoroutine(DelayedExplosionCoroutine(ExplosionDelay));
    }

    private IEnumerator DelayedExplosionCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        Entity.DamageEntitiesWithAOELaunch(enemy, ExplosionRadius, ExplosionPercentDamage, ExplosionLaunchForce, ExplosionStunDuration);
        //insert explosion vfx here:
        CustomDebug.InstantiateTemporarySphere(enemy.transform.position, ExplosionRadius, 1f, new Color(1f, 0, 0, 0.2f));
    }
}
