using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldEventQuestSO : ProgressionQuestSO
{
  [Header("World Event Criteria")]
  [SerializeField] private WorldEventSO requiredWorldEvent;

  protected EventManager eventManager;

  // For World Event Quests, the Current Event must be the one that's required in order for them to be selectable options for the progressionManager.
  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    eventManager = progressionManager?.eventManager;

    if (eventManager == null)
      return false;
    
    return progressionManager.eventManager.CurrentEvent.GetType() == requiredWorldEvent.GetType();
  }
}
