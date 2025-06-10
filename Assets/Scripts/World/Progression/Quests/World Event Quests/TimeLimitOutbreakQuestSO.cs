using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_SecondsOutbreakQuestSO", menuName = "World/Progression Quest/World Event Quests/Time Limit (Outbreak)")]
public class TimeLimitOutbreakQuestSO : WorldEventQuestSO
{
  [field: Header("Time Limit (Outbreak) Configuration")]

  /// <summary>
  /// The allotted amount of seconds in which the player must complete the wave to complete the quest, multiplied by the number of lands selected by the event.
  /// </summary>
  /// <value></value>
  [field: Tooltip("The allotted amount of seconds in which the player must complete the wave to complete the quest, multiplied by the number of lands selected by the event.")]
  [field: Range(10, 60)]
  [field: SerializeField] public int SecondsPerOutbreakLand { get; private set; }

  /// <summary>
  /// Reference to the Outbreak World Event via the Event Manager.
  /// </summary>
  private ZonesWorldEventSO outbreakEvent;
  
  /// <summary>
  /// the time remaining, in seconds, for the player to complete the wave.
  /// </summary>
  private float remainingTime = 0f;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    if (!progressionManager.eventManager)
    {
      if (LogErrorMessages) 
        Debug.LogError($"{name} Criteria Error: Could not find reference to the Event Manager.");

      return false;
    }

    // Assign the references to the corresponding variables.
    eventManager ??= progressionManager.eventManager;

    if (eventManager.CurrentEvent is not ZonesWorldEventSO)
    {
      {
        if (LogErrorMessages)
          Debug.LogError($"{name} Criteria Error: Required World Event is not of type {typeof(ZonesWorldEventSO)}.");

        return false;  
      }
    }

    // Assign the references to the corresponding variables.
    outbreakEvent ??= (ZonesWorldEventSO)eventManager.CurrentEvent;

    return true;
  }

  private protected override void OnActivated()
  {
    // Set the remaining time.
    remainingTime = outbreakEvent.affectedLands.Count * SecondsPerOutbreakLand;

    // Update the quest's objective text.
    ObjectiveText = $"Complete the wave objective within the remaining time: {TimeSpan.FromSeconds(remainingTime).ToString(@"mm\:ss")}";
  }

  private protected override void OnCleanUp()
  {
    // Check for completion at the end of the wave.
    if (remainingTime > 0f)
      Complete(false);

    // Reset the quest's objective text.
    ObjectiveText = $"Complete the wave objective within the remaining time: ";
  }

  private protected override void OnUpdate()
  {
    if (progressionManager.gameManager.CurrentState == GameState.PLAYING)
    {
      remainingTime -= Time.unscaledDeltaTime;
      remainingTime = Mathf.Clamp(remainingTime, 0f, remainingTime);
      ObjectiveText = $"Complete the wave objective within the remaining time: {TimeSpan.FromSeconds(remainingTime).ToString(@"mm\:ss")}";
    }
  }
}
