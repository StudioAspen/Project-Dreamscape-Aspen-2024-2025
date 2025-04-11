using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Fill_x_MemoryMeterQuestSO", menuName = "World/Progression Quest/Skillful Quests/Fill Memory Meter")]
public class FillMemoryMeterQuestSO : SkillfulQuestSO
{
  [field: Header("Fill Memory Meter Configuration")]

  /// <summary>
  /// The minimum Memory Meter level that the player must achieve to complete the quest.
  /// </summary>
  [field: Tooltip("The minimum Memory Meter level that the player must achieve to complete the quest.")]
  [field: Range (0, 3)]
  [field: SerializeField] public int MemoryLevelGoal { get; private set; }

  /// <summary>
  /// The minimum number of unique Shard Types with which the player must fill the Memory Meter to complete the quest.
  /// </summary>
  [field: Tooltip("The minimum unique Shard Types with which the player must fill the Memory Meter to complete the quest.")]
  [field: Range (0, 6)]
  [field: SerializeField] public int ShardTypesGoal { get; private set; }

  /// <summary>
  /// Did the player meet the minimum Memory Level goal?
  /// </summary>
  private bool memoryLevelGoalMet = false;

  /// <summary>
  /// Did the player meet the minimum unique Shard Types goal?
  /// </summary>
  private bool shardTypesGoalMet = false;

  /// <summary>
  /// Reference to the Memory System via the Progression Manager and Player.
  /// </summary>
  private MemorySystem memorySystem;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    if (!progressionManager.player.TryGetComponent(out MemorySystem memorySystemRef))
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find Memory System component on the player.");

      return false;
    }
    else if (memorySystemRef.GetMemoryLevel() >= MemoryLevelGoal && ShardTypesGoal == 0)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Player has already filled the Memory Meter to the minimum Level goal.");

      return false;
    }
    else if (memorySystemRef.ShardDictionary.Count >= ShardTypesGoal && MemoryLevelGoal == 0)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Player has already filled the Memory Meter with the minimum unique Shard Types goal.");

      return false;
    }

    // Assign the Memory System reference to the variable.
    memorySystem ??= memorySystemRef;
    
    // Check the base criteria.
    return base.MeetsCriteria(progressionManager);
  }

  private protected override void OnActivated()
  {
    // Subscribe to the necessary Actions.
    memorySystem.OnShardAdded += MemoryMeter_ShardAdded;
    memorySystem.OnNewShardTypeAdded += MemoryMeter_NewShardType;
  }

  private protected override void OnCleanUp()
  {
    // Unsubscribe to any Actions used for the quest.
    memorySystem.OnShardAdded -= MemoryMeter_ShardAdded;
    memorySystem.OnNewShardTypeAdded -= MemoryMeter_NewShardType;
  }

  private protected override void OnUpdate() { }

  private void MemoryMeter_ShardAdded(string shardHolderType)
  {
    // Check if the Memory Meter meets minimum memory level goal.
    memoryLevelGoalMet = memorySystem.GetMemoryLevel() >= MemoryLevelGoal;

    if (memoryLevelGoalMet && shardTypesGoalMet)
      Complete();
  }

  private void MemoryMeter_NewShardType(string shardHolderType)
  {
    // Check if the Shard dictionary count meets the unique Shard Types goal.
    shardTypesGoalMet = memorySystem.ShardDictionary.Count >= ShardTypesGoal;

    if (memoryLevelGoalMet && shardTypesGoalMet)
      Complete();
  }
}
