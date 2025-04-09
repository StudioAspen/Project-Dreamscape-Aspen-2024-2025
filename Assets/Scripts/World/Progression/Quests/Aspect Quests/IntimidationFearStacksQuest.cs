using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_FearStacksByIntimidationQuestSO", menuName = "World/Progression Quest/Aspect Quests/Indimidation Fear Stacks")]
public class IntimidationFearStacksQuest : AspectQuestSO
{
  [field: Header("Indimidation Fear Stacks Configuration")]

  /// <summary>
  /// The minimum Fear stacks the player must achieve with the Intimidation Passive Ability to complete the quest.
  /// </summary>
  [Tooltip("The minimum Fear stacks the player must achieve with the Intimidation Passive Ability to complete the quest.")]
  [field: Range(0, 5)]
  [field: SerializeField] public int FearStacksGoal { get; private set; }

  /// <summary>
  /// The number of Fear stacks the player has achieved with the Intimidation Passive Ability.
  /// </summary>
  private int fearStacks = 0;

  /// <summary>
  /// Reference to the Player via the Progression Manager.
  /// </summary>
  private Player player;

  /// <summary>
  /// Reference to the Player Status Effector via the Progression Manager and Player.
  /// </summary>
  private EntityStatusEffector playerStatusEffector;

  /// <summary>
  /// Reference to the Intimidation Passive Ability via the Player Status Effector.
  /// </summary>
  private AspectOfFearPassiveBStatusEffectSO playerIntimidationPassive;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    if (!progressionManager.player)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the player.");

      return false;
    }

    player ??= progressionManager.player;

    if (!player.TryGetComponent(out EntityStatusEffector entityStatusEffectorRef))
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find Player Status Effector component on the player.");

      return false;  
    }
    else if (!entityStatusEffectorRef.TryGetStatusEffect<AspectOfFearPassiveBStatusEffectSO>())
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the Intimidation Passive Ability.");

      return false;
    }

    // Assign the references to the corresponding variables.
    playerStatusEffector ??= entityStatusEffectorRef;
    playerIntimidationPassive ??= entityStatusEffectorRef.TryGetStatusEffect<AspectOfFearPassiveBStatusEffectSO>();

    if (playerIntimidationPassive.CurrentStacks >= FearStacksGoal)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Player has already achieved the minimum Fear stacks goal.");

      return false;
    }

    // Check the base criteria.
    return base.MeetsCriteria(progressionManager);
  }

  // Subscribe to the necessary Actions.
  private protected override void OnActivated() => playerIntimidationPassive.OnStunEntity += PlayerIntimidation_OnEntityStunned;

  // Unsubscribe to any Actions used for the quest.
  private protected override void OnCleanUp() => playerIntimidationPassive.OnStunEntity -= PlayerIntimidation_OnEntityStunned;

  private protected override void OnUpdate() { }

  private void PlayerIntimidation_OnEntityStunned(Entity stunner, Entity victim, float stunDuration)
  {
    fearStacks = playerIntimidationPassive.CurrentStacks;
    
    // Check if the updated Fear stacks meets the goal.
    if (fearStacks >= FearStacksGoal)
      Complete();
  }
}
