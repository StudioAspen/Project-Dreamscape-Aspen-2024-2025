using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Duplicator Elite Variant", menuName = "Status Effect/Enemy Variants/Elite/Duplicator")]
public class DuplicatorEliteVariantStatusEffectSO : EliteVariantStatusEffectSO
{
    [field: Header("Config")]
    [field: SerializeField] public int SplitCount { get; private set; } = 2;
    private Enemy enemyPrefab;

    private protected override void OnApply()
    {
        base.OnApply();

        enemyPrefab = enemy.Spawner.GetPrefabFromEnemyInstance(enemy);

        enemy.OnEntityDeath += Enemy_OnEntityDeath;
    }

    public override void Cancel()
    {
        base.Cancel();

        enemy.OnEntityDeath -= Enemy_OnEntityDeath;
    }

    private void Enemy_OnEntityDeath(GameObject killer)
    {
        if (enemyPrefab == null) return;

        // Spawn neutral duplicates in a circle around the original enemy
        for (int i = 0; i < SplitCount; i++)
        {
            // Calculate the angle between each duplicate
            float angle = i * (360f / SplitCount);

            // Calculate the position of the duplicate around the original enemy
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad)) * 1f;

            // Spawn the enemy around the original enemy in a circle with SplitCount points with a radius of 1
            Vector3 spawnPosition = enemy.transform.position + offset;
            enemy.Spawner.SpawnEnemy(enemyPrefab, spawnPosition);
        }
    }
}
