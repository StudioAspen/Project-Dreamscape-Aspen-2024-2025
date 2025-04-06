using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_RemainingEscortQuestSO", menuName = "World/Progression Quest/World Event Quests/Remaining Health (Escort)")]
public class RemainingHealthEscortQuestSO : WorldEventQuestSO
{
  [field: Header("Remaining Health (Escort) Configuration")]

  /// <summary>
  /// The minimum percentage of the Escort Entity's max health that the player must preserve until the end of the wave to complete the quest.
  /// </summary>
  [field: Tooltip("The minimum percentage of the Escort Entity's max health that the player must preserve until the end of the wave to complete the quest.")]
  [field: Range(0.01f, 1.00f)]
  [field: SerializeField] private float minimumHealthPercentage;

  /// <summary>
  /// Reference to the Escort World Event via the Event Manager.
  /// </summary>
  private EscortWorldEventSO escortEvent;

  /// <summary>
  /// Reference to the Escort Entity via the Event Manager and the Escort World Event.
  /// </summary>
  private EscortEventEntity escortEntity;

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

    if (eventManager.CurrentEvent is not EscortWorldEventSO)
    {
      {
        if (LogErrorMessages)
          Debug.LogError($"{name} Criteria Error: Required World Event is not of type {typeof(EscortWorldEventSO)}.");

        return false;  
      }
    }

    // Assign the references to the corresponding variables.
    escortEvent ??= (EscortWorldEventSO)eventManager.CurrentEvent;

    return true;
  }

  // Find the spawned Escort Entity.
  private protected override void OnActivated() => escortEntity ??= FindFirstObjectByType<EscortEventEntity>();

  private protected override void OnCleanUp()
  {
    // Calculate the minimum remaining health required to complete the quest as an integer
    int minimumCurrentHealth = Mathf.FloorToInt(escortEntity.MaxHealth.BaseValue * minimumHealthPercentage);

    // Check for completion at the end of the wave.
    if (escortEntity.CurrentHealth >= minimumCurrentHealth)
      Complete(false);
  }

  private protected override void OnUpdate() { }
}