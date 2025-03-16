using UnityEngine;
using System.Collections;


[CreateAssetMenu(fileName = "Complete Wave Under X * LandCount Time Quest", menuName = "World/Progression Quest/Complete Wave Under X LandCount Time")]
public class CompleteWaveUnderXLandCountTimeQuestSO : ProgressionQuestSO
{
    private WorldManager worldManager;

    [field: Header("Config")]
    [field: SerializeField] public float TimeMultiplier { get; private set; } = 60f;

    private float requiredTime;
    private float timer;

    private protected override void OnActivated()
    {
        worldManager = FindObjectOfType<WorldManager>();

        requiredTime = TimeMultiplier * worldManager.SpawnedLands.Count;
        timer = 0f;
    }

    private protected override void OnCleanUp()
    {
        if(timer < requiredTime)
        {
            CompleteWithoutCleanUp();
        }
    }

    private protected override void OnUpdate()
    {
        timer += Time.deltaTime;
    }
}
