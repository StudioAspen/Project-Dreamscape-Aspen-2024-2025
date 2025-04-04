using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Perform_x_MemoryAbilityQuestSO", menuName = "World/Progression Quest/Skillful Quests/Perform Memory Ability")]
public class MemoryAbilityQuestSO : SkillfulQuestSO
{
  [field: Header("Perform Memory Ability Configuration")]

  /// <summary>
  /// The minimum Memory Meter level required for the Progression Manager to select this quest.
  /// </summary>
  [field: Tooltip("The minimum Memory Meter level required for the Progression Manager to select this quest.")]
  [field: Range(0, 3)]
  [field: SerializeField] public int MinRequiredMemoryLevel { get; private set; }

  /// <summary>
  /// The Memory Ability required to complete this quest. The Progression Manager will check if the majority of the Memory Meter consists of Shards for the specified Memory Ability.
  /// </summary>
  [field: Tooltip("The Memory Ability required to complete this quest. The Progression Manager will check if the majority of the Memory Meter consists of Shards for the specified Memory Ability.")]
  [field: SerializeField] public string RequiredMemoryAbility { get; private set; }

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
    else if (memorySystemRef.GetMemoryLevel() < MinRequiredMemoryLevel)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Memory Meter Level is below the minimum requirement.");

      return false;
    }
    else if (string.IsNullOrEmpty(RequiredMemoryAbility) || !memorySystemRef.ShardDictionary.ContainsKey(RequiredMemoryAbility))
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: A required Memory Ability that exists was not provided.");

      return false;
    }
    else if (memorySystemRef.GetLargestShardType() != RequiredMemoryAbility)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Majority of Memory Meter does not consist of the Shards for the specified Memory Ability.");
      
      return false;
    }
    
    // Assign the Memory System reference to the variable.
    memorySystem ??= memorySystemRef;

    // Check the base criteria.
    return base.MeetsCriteria(progressionManager);
  }

  // Subscribe to the necessary Actions.
  private protected override void OnActivated() => memorySystem.OnMemoryAbilityActivated += MemoryMeter_AbilityActivated;

  // Unsubscribe to any Actions used for the quest.
  private protected override void OnCleanUp() => memorySystem.OnMemoryAbilityActivated -= MemoryMeter_AbilityActivated;

  private protected override void OnUpdate() { }

  private void MemoryMeter_AbilityActivated(string shardHolderType)
  {
    // Check if the Memory Ability is the required one.
    if (shardHolderType == RequiredMemoryAbility)
      Complete();
  }
}
