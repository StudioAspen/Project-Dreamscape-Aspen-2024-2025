using UnityEngine;

public abstract class WorldEventQuestSO : ProgressionQuestSO
{
  [field: Header("World Event Criteria")]
  /// <summary>
  /// The World Event required to be in effect for the Progression Manager to select this quest.
  /// </summary>
  [field: Tooltip("The World Event required to be in effect for the Progression Manager to select this quest.")]
  [field: SerializeField] protected WorldEventSO requiredWorldEvent;

  /// <summary>
  /// Reference to the Event Manager via the Progression Manager.
  /// </summary>
  protected EventManager eventManager;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    if (!progressionManager.eventManager)
    {
      if (LogErrorMessages) 
        Debug.LogError($"{name} Criteria Error: Could not find reference to the Event Manager.");

      return false;
    }
    
    // Assign the Event Manager reference to the variable.
    eventManager ??= progressionManager.eventManager;
    
    // The quest references are two different instances of the same type. We need to check if they share the same type, not if they're the same instance.
    return eventManager.CurrentEvent.GetType() == requiredWorldEvent.GetType();
  }
}
