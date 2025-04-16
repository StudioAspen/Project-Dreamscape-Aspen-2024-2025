using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_RemainingDefendQuestSO", menuName = "World/Progression Quest/World Event Quests/Remaining Health (Defend)")]
public class RemainingHealthDefendQuestSO : WorldEventQuestSO
{
  [field: Header("Remaining Health (Defend) Configuration")]

  /// <summary>
  /// The minimum percentage of the Defend Entity's max health that the player must preserve until the end of the wave to complete the quest.
  /// </summary>
  [field: Tooltip("The minimum percentage of the Defend Entity's max health that the player must preserve until the end of the wave to complete the quest.")]
  [field: Range(0.01f, 1.00f)]
  [field: SerializeField] private float minimumHealthPercentage;

  /// <summary>
  /// Reference to the Defend World Event via the Event Manager.
  /// </summary>
  private DefendWorldEventSO defendEvent;

  /// <summary>
  /// Reference to the Defend Entity via the Event Manager and the Defend World Event.
  /// </summary>
  private DefendEventEntity defendEntity;

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

    if (eventManager.CurrentEvent is not DefendWorldEventSO)
    {
      {
        if (LogErrorMessages)
          Debug.LogError($"{name} Criteria Error: Required World Event is not of type {typeof(DefendWorldEventSO)}.");

        return false;  
      }
    }

    // Assign the references to the corresponding variables.
    defendEvent ??= (DefendWorldEventSO)eventManager.CurrentEvent;

    return true;
  }

  // Find the spawned Defend Entity.
  private protected override void OnActivated() => defendEntity ??= FindFirstObjectByType<DefendEventEntity>();

  private protected override void OnCleanUp()
  {
    // Calculate the minimum remaining health required to complete the quest as an integer
    int minimumCurrentHealth = Mathf.FloorToInt(defendEntity.MaxHealth.BaseValue * minimumHealthPercentage);

    // Check for completion at the end of the wave.
    if (defendEntity.CurrentHealth >= minimumCurrentHealth)
      Complete(false);
  }

  private protected override void OnUpdate() { }
}