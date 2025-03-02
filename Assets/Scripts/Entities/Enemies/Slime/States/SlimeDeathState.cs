using UnityEngine;

[System.Serializable]
public class SlimeDeathState : EntityDeathState
{
    private Slime slime;

    private float currentTime;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        slime = entity as Slime;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();

        // small slimes dont split, they die
        if (slime.IsSmall) return;

        Enemy slimeEnemyPrefab = slime.Spawner.GetPrefabFromEnemyInstance(slime);
        if (slimeEnemyPrefab == null) return;

        for (int i = 0; i < slime.SplitCount; i++)
        {
            // if you suspect this is crashing game uncomment bellow
            // Debug.Break();
            float angle = i * (360f / slime.SplitCount);

            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));

            Vector3 spawnPos = slime.transform.position + offset;

            Slime duplicateSlime = slime.Spawner.SpawnEnemy(slimeEnemyPrefab, spawnPos) as Slime;
            if (duplicateSlime == null)
            {
                Debug.LogWarning("Duplicate slime is null");
                continue;
            }
            duplicateSlime.SetSmall(true);
            duplicateSlime.HealToFull(false);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
