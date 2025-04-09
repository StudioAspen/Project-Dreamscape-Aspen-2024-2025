using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Gain_x_MomentumQuestSO", menuName = "World/Progression Quest/Skillful Quests/Gain Momentum")]
public class GainMomentumQuestSO : SkillfulQuestSO
{
  [field: Header("Gain Momentum Configuration")]
  /// <summary>
  /// The minimum Momentum that the player must gain to complete the quest.
  /// </summary>
  [field: Tooltip("The minimum Momentum that the player must gain to complete the quest.")]
  [field: Range(1, 10)]
  [field: SerializeField] public int MomentumGoal { get; private set; }

  /// <summary>
  /// Reference to the momentum System via the Progression Manager and Player.
  /// </summary>
  private MomentumSystem momentumSystem;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    if (!progressionManager.player.TryGetComponent(out MomentumSystem momentumSystemRef))
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find Momentum System component on the player.");

      return false;
    }
    else if (momentumSystemRef.Momentum >= MomentumGoal)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Player has already achieved the minimum Momentum goal.");

      return false;
    }
    
    // Assign the Momentum System reference to the variable.
    momentumSystem ??= momentumSystemRef;

    // Check the base criteria.
    return base.MeetsCriteria(progressionManager);
  }

  // Subscribe to the necessary Actions.
  private protected override void OnActivated() => momentumSystem.OnMomentumUpdated += CompareMomentumUpdate;

  // Unsubscribe to any Actions used for the quest.
  private protected override void OnCleanUp() => momentumSystem.OnMomentumUpdated -= CompareMomentumUpdate;

  private protected override void OnUpdate() { }

  private void CompareMomentumUpdate(int newMomentum)
  {
    // Check if the updated momentum meets the goal.
    if (newMomentum >= MomentumGoal)
      Complete();
  }
}
