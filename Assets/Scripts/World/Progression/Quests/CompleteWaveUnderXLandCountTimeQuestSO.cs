using UnityEngine;

[CreateAssetMenu(fileName = "Complete Wave Under X * LandCount Time Quest", menuName = "World/Progression Quest/Complete Wave Under X LandCount Time")]
public class CompleteWaveUnderXLandCountTimeQuestSO : ProgressionQuestSO
{
    private EnemySpawner[] enemySpawners;
    private WorldManager worldManager;

    [field: Header("Config")]
    [field: SerializeField] public float TimeMultiplier { get; private set; } = 60f;

    private float requiredTime;
    private int activeSpawners;

    private protected override void OnActivated()
    {
        worldManager = FindObjectOfType<WorldManager>();
        enemySpawners = FindObjectsOfType<EnemySpawner>();

        int landCount = worldManager.SpawnedLands.Count;
        requiredTime = TimeMultiplier * landCount;
        activeSpawners = enemySpawners.Length;

        foreach (var spawner in enemySpawners)
        {
            spawner.OnSpawnerDepleted += HandleSpawnerDepletion;
        }
    }

    private protected override void OnCleanUp()
    {
        foreach (var spawner in enemySpawners)
        {
            spawner.OnSpawnerDepleted -= HandleSpawnerDepletion;
        }
    }

    private protected override void OnUpdate()
    {
    }

    private void HandleSpawnerDepletion()
    {
        activeSpawners--;

        if (activeSpawners <= 0)
        {
            float waveCompletionTime = Time.timeSinceLevelLoad;

            if (waveCompletionTime <= requiredTime)
            {
                Complete();
            }
        }
    }
}
