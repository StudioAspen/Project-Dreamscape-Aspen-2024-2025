using UnityEngine;

[CreateAssetMenu(fileName = "Complete Wave Under X * LandCount Time Quest", menuName = "World/Progression Quest/Complete Wave Under X LandCount Time")]
public class CompleteWaveUnderXLandCountTimeQuestSO : ProgressionQuestSO
{
    private EnemySpawner[] enemySpawners;
    private WorldManager worldManager;

    [field: Header("Config")]
    [field: SerializeField] public float TimeMultiplier { get; private set; } = 60f;

    private float requiredTime;

    private protected override void OnActivated()
    {
        worldManager = FindObjectOfType<WorldManager>();
        enemySpawners = FindObjectsOfType<EnemySpawner>();

        int landCount = worldManager.SpawnedLands.Count;
        requiredTime = TimeMultiplier * landCount;

        foreach (var spawner in enemySpawners)
        {
            spawner.OnSpawnerDepleted += TrackWaveCompletion;
        }
    }

    private protected override void OnCleanUp()
    {
        foreach (var spawner in enemySpawners)
        {
            spawner.OnSpawnerDepleted -= TrackWaveCompletion;
        }
    }

    private protected override void OnUpdate()
    {
    }

    private void TrackWaveCompletion()
    {
        float waveCompletionTime = Time.timeSinceLevelLoad; 

        if (waveCompletionTime <= requiredTime)
        {
            Complete();
        }
    }
}
