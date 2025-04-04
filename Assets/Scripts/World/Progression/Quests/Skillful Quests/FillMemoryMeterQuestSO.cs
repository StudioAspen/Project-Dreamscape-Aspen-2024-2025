using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fill_x_MemoryMeterQuestSO", menuName = "World/Progression Quest/Skillful Quests/Fill Memory Meter")]
public class FillMemoryMeterQuestSO : SkillfulQuestSO
{
  private MemorySystem memorySystem;

  [field: Header("Config")]
  [field: Range (0, 3)]
  [field: SerializeField] public int MemoryLevelGoal { get; private set; }
  [field: Range (0, 6)]
  [field: SerializeField] public int ShardTypesGoal { get; private set; }

  private bool memoryLevelGoalMet;
  private bool shardTypesGoalMet;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    memorySystem = FindFirstObjectByType<MemorySystem>();

    if (memorySystem == null)
      return false;
    
    if (memorySystem.GetMemoryLevel() >= MemoryLevelGoal && ShardTypesGoal == 0)
      return false;
    else if (memorySystem.ShardDictionary.Count >= ShardTypesGoal && MemoryLevelGoal == 0)
      return false;
    
    return base.MeetsCriteria(progressionManager);
  }
  private protected override void OnActivated()
  {
    memoryLevelGoalMet = memorySystem.GetMemoryLevel() >= MemoryLevelGoal;
    shardTypesGoalMet = memorySystem.ShardDictionary.Count >= ShardTypesGoal;
    if (memorySystem == null)
    {
      CleanUp();
      return;
    }

    memorySystem.OnNewShardTypeAdded += MemoryMeter_NewShardType;
    memorySystem.OnShardAdded += MemoryMeter_ShardAdded;
  }

  private protected override void OnCleanUp()
  {
    if (memorySystem == null)
      return;

    memorySystem.OnNewShardTypeAdded -= MemoryMeter_NewShardType;
    memorySystem.OnShardAdded -= MemoryMeter_ShardAdded;
  }

  private protected override void OnUpdate()
  {
    if(memoryLevelGoalMet && shardTypesGoalMet)
      Complete();
  }

  private void MemoryMeter_ShardAdded(string shardHolderType) => memoryLevelGoalMet = memorySystem.GetMemoryLevel() >= MemoryLevelGoal;

  private void MemoryMeter_NewShardType(string shardHolderType) => shardTypesGoalMet = memorySystem.ShardDictionary.Count >= ShardTypesGoal;
}
