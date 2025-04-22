using UnityEngine;

[CreateAssetMenu(fileName = "Preserve_x_PercentSurvivalQuestSO", menuName = "World/Progression Quest/World Event Quests/Preserve Health Percentage (Survival)")]
public class PreserveHealthSurvivalQuest : WorldEventQuestSO
{
  [field: Header("Preserve Health Percentage (Survival) Configuration")]

  /// <summary>
  /// The minimum percentage of the player's remaining health that they must preserve until the end of the wave to complete the quest.
  /// </summary>
  [field: Tooltip("The minimum percentage of the player's remaining health that they must preserve until the end of the wave to complete the quest.")]
  [field: Range(0.01f, 1.00f)]
  [field: SerializeField] private float minimumHealthPercentage;

  /// <summary>
  /// Reference to the Player via the Progression Manager.
  /// </summary>
  private Player player;

  /// <summary>
  /// The amount of remaining health the player had when the wave started.
  /// </summary>
  private int startingHealth = 0;

  /// <summary>
  /// Reference to the Survival World Event via the Event Manager.
  /// </summary>
  private SurvivalWorldEventSO survivalEvent;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    if (!progressionManager.player)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the player.");

      return false;
    }
    else if (!progressionManager.eventManager)
    {
      if (LogErrorMessages) 
        Debug.LogError($"{name} Criteria Error: Could not find reference to the Event Manager.");

      return false;
    }
    
    // Assign the references to the corresponding variables.
    player ??= progressionManager.player;
    eventManager ??= progressionManager.eventManager;

    if(eventManager.CurrentEvent is not SurvivalWorldEventSO)
    {
      {
        if (LogErrorMessages)
          Debug.LogError($"{name} Criteria Error: Required World Event is not of type {typeof(SurvivalWorldEventSO)}.");

        return false;  
      }
    }

    // Assign the references to the corresponding variables.
    survivalEvent ??= (SurvivalWorldEventSO)eventManager.CurrentEvent;

    return true;
  }

  // Get the player's current health.
  private protected override void OnActivated() => startingHealth = player.CurrentHealth;


  private protected override void OnCleanUp()
  {
    // Calculate the minimum remaining health required to complete the quest as an integer
    int minimumCurrentHealth = Mathf.FloorToInt(startingHealth * minimumHealthPercentage);

    // Check for completion at the end of the wave.
    if(player.CurrentHealth >= minimumCurrentHealth)
      Complete(false);
  }

  private protected override void OnUpdate() { }
}
