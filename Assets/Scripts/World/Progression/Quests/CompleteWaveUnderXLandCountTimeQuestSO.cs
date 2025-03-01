using UnityEngine;
using System.Collections;


[CreateAssetMenu(fileName = "Complete Wave Under X * LandCount Time Quest", menuName = "World/Progression Quest/Complete Wave Under X LandCount Time")]
public class CompleteWaveUnderXLandCountTimeQuestSO : ProgressionQuestSO
{
    private WorldManager worldManager;
    private EventManager eventManager;

    [field: Header("Config")]
    [field: SerializeField] public float TimeMultiplier { get; private set; } = 60f;

    private float requiredTime;
    private Coroutine timerCoroutine;
    private bool timeExpired = false;

    private protected override void OnActivated()
    {
        worldManager = FindObjectOfType<WorldManager>();
        eventManager = FindObjectOfType<EventManager>();
        requiredTime = TimeMultiplier * worldManager.SpawnedLands.Count;

        timerCoroutine = worldManager.StartCoroutine(TimerCountdown());

        eventManager.OnEventCleared += HandleEventCleared;
    }

    private IEnumerator TimerCountdown()
    {
        yield return new WaitForSeconds(requiredTime);
        timeExpired = true; // Mark time as expired
  
    }

    private void HandleEventCleared(WorldEventSO worldEvent)
    {
        if (!timeExpired) // Only complete if time has NOT expired
        {
            Complete();
        }
    }

    private protected override void OnCleanUp()
    {
        if (timerCoroutine != null)
        {
            worldManager.StopCoroutine(timerCoroutine);
        }

        eventManager.OnEventCleared -= HandleEventCleared;
    }

    private protected override void OnUpdate()
    {
    }
}
