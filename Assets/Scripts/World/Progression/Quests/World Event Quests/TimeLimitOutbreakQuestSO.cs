using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_SecondsOutbreakQuestSO", menuName = "World/Progression Quest/World Event Quests/Time Limit (Outbreak)")]
public class TimeLimitOutbreakQuestSO : WorldEventQuestSO
{
    [field: Header("Config")]
    [field: Range(10, 60)]
    [field: SerializeField] public int SecondsPerOutbreakLand { get; private set; }
    private ZonesWorldEventSO outbreakEventRef;
    private float startingTimer = 0f;

  private protected override void OnActivated()
  {
    outbreakEventRef = (ZonesWorldEventSO)eventManager?.CurrentEvent;
    
    if (outbreakEventRef == null)
    {
      CleanUp();
      return;
    }

    startingTimer = outbreakEventRef.affectedLands.Count * SecondsPerOutbreakLand;
    ObjectiveText = $"Complete the wave objective within the remaining time: {TimeSpan.FromSeconds(startingTimer).ToString(@"mm\:ss")}";
  }

  private protected override void OnCleanUp()
  {
    if (startingTimer <= 0f)
      return;
    else
      Complete(false);

    ObjectiveText = $"Complete the wave objective within the remaining time: ";
  }

  private protected override void OnUpdate()
  {
    if (progressionManager.gameManager.CurrentState == GameState.PLAYING)
    {
        startingTimer -= Time.unscaledDeltaTime;
        startingTimer = Mathf.Clamp(startingTimer, 0f, startingTimer);
        ObjectiveText = $"Complete the wave objective within the remaining time: {TimeSpan.FromSeconds(startingTimer).ToString(@"mm\:ss")}";
    }
  }
}
