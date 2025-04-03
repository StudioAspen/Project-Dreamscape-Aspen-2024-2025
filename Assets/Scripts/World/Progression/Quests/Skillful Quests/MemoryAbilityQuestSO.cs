using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Perform_x_MemoryAbilityQuestSO", menuName = "World/Progression Quest/Skillful Quests/Perform Memory Ability")]
public class MemoryAbilityQuestSO : SkillfulQuestSO
{
  private MemorySystem memorySystem;

  [field: Header("Config")]
  [field: SerializeField] public bool RequireFullBar { get; private set; }
  [field: Range(0, 3)]
  [field: SerializeField] public int MinRequiredMemoryLevel { get; private set; }
  [field: SerializeField] public string RequiredMemoryAbility { get; private set; }
  private bool memoryMeterFull = false;
  private bool memoryAbilityActivated = false;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    memorySystem = FindFirstObjectByType<MemorySystem>();

    // For Quest of this class to meet the Criteria, the memory meter should contain the majority of shards for the specified Memory Ability.
    if (memorySystem.GetMemoryLevel() < MinRequiredMemoryLevel)
      return false;
    else if (RequiredMemoryAbility != string.Empty)
      if (memorySystem.GetLargestShardType() != RequiredMemoryAbility || !memorySystem.ShardDictionary.ContainsKey(RequiredMemoryAbility))
        return false;
    
    return base.MeetsCriteria(progressionManager);
  }

  private protected override void OnActivated()
  {
    memorySystem = FindFirstObjectByType<MemorySystem>();
    if (memorySystem == null)
    {
      CleanUp();
      return;
    }

    // Check if the meter is already full;
    memoryMeterFull = memorySystem.GetMemoryLevel() == 3;

    memorySystem.OnMemoryBarFull += MemoryMeter_Full;
    memorySystem.OnMemoryAbilityActivated += MemoryMeter_AbilityActivated;
  }


  private protected override void OnCleanUp()
  {
    if (memorySystem == null)
      return;

    memorySystem.OnMemoryBarFull -= MemoryMeter_Full;
    memorySystem.OnMemoryAbilityActivated -= MemoryMeter_AbilityActivated;
  }

  private protected override void OnUpdate()
  {
    if (RequireFullBar && memoryMeterFull)
      Complete();
    else if(memoryAbilityActivated)
      Complete();
  }


  private void MemoryMeter_Full(string shardHolderType)
  {
    if(memoryMeterFull == false && RequireFullBar)
      memoryMeterFull = true;
  }

  private void MemoryMeter_AbilityActivated(string shardHolderType)
  {
    if (shardHolderType == RequiredMemoryAbility)
      memoryAbilityActivated = true;
    else if (RequiredMemoryAbility == string.Empty)
      memoryAbilityActivated = true;
  }
}
