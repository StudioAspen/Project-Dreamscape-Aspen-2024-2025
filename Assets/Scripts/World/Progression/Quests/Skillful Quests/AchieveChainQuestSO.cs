using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Achieve_x_ChainQuestSO", menuName = "World/Progression Quest/Skillful Quests/Achieve Chain")]
public class AchieveChainQuestSO : SkillfulQuestSO
{
  [field: Header("Achieve Chain Configuration")]

  /// <summary>
  /// The minimum Chain that the player must achieve to complete the quest.
  /// </summary>
  [field: Tooltip("The minimum Chain that the player must achieve to complete the quest.")]
  [field: Range(1, 100)]
  [field: SerializeField] public int ChainGoal { get; private set; }

  /// <summary>
  /// Reference to the Chaining System via the Progression Manager and Player.
  /// </summary>
  private ChainingSystem chainingSystem;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    if (!progressionManager.player.TryGetComponent(out ChainingSystem chainingSystemRef))
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find Chaining System component on the player.");

      return false;
    }
    else if (chainingSystemRef.ChainCount >= ChainGoal)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Player has already achieved the minimum Chain goal.");

      return false;
    }
    
    // Assign the Chaining System reference to the variable.
    chainingSystem ??= chainingSystemRef;

    // Check the base criteria.
    return base.MeetsCriteria(progressionManager);
  }

  // Subscribe to the necessary Actions.
  private protected override void OnActivated() => chainingSystem.OnChainUpdated += CompareChainUpdate;
  
  // Unsubscribe to any Actions used for the quest.
  private protected override void OnCleanUp() => chainingSystem.OnChainUpdated -= CompareChainUpdate;

  private protected override void OnUpdate() { }

  private void CompareChainUpdate(int newChainCount)
  {
    // Check if the updated chain count meets the goal.
    if (newChainCount >= ChainGoal)
      Complete();
  }
}
