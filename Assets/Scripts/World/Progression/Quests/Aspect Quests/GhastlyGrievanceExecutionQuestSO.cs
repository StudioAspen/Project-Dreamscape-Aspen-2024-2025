using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_Times_y_ByGhastlyGrievanceQuestSO", menuName = "World/Progression Quest/Aspect Quests/Ghastly Grievance Execution")]
public class GhastlyGrievanceExecutionQuestSO : AspectQuestSO
{
  [field: Header("Ghastly Grievance Execution Configuration")]

  /// <summary>
  /// The number of times the player must afflict Dreamons with the Ghastly Grievance status effect to complete the quest.
  /// </summary>
  [Tooltip("The number of times the player must afflict Dreamons with the Ghastly Grievance status effect to complete the quest.")]
  [field: Range(0, 15)]
  [field: SerializeField] public int SuccessfulHitsGoal { get; private set; }

  /// <summary>
  /// The number of times the player must execute Dreamons with Ghastly Grievance to complete the quest.
  /// </summary>
  [Tooltip("The number of times the player must execute Dreamons with Ghastly Grievance to complete the quest.")]
  [field: Range(0, 5)]
  [field: SerializeField] public int SuccessfulExecutionsGoal { get; private set; }

  /// <summary>
  /// The number of times the player has executed Dreamons with Ghastly Grievance.
  /// </summary>
  private int successfulExecutions = 0;

  /// <summary>
  /// Reference to the Player via the Progression Manager.
  /// </summary>
  private Player player;

  /// <summary>
  /// Reference to the Player Status Effector via the Progression Manager and Player.
  /// </summary>
  private EntityStatusEffector playerStatusEffector;

  /// <summary>
  /// Reference to the Ghastly Grievance Passive Ability via the Player Status Effector.
  /// </summary>
  private AspectOfFearPassiveAStatusEffectSO playerGhastlyGrievancePassive;

  /// <summary>
  /// List that tracks every applied Ghastly Grievance status effect during the quest.
  /// </summary>
  /// <typeparam name="ExtendedDebuffStatusEffectSO">The Ghastly Grievance status effect applied to a Dreamon.</typeparam>
  private List<ExtendedDebuffStatusEffectSO> appliedGhastlyGrievances = new List<ExtendedDebuffStatusEffectSO>();

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
    else if (!entityStatusEffectorRef.TryGetStatusEffect<AspectOfFearPassiveAStatusEffectSO>())
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the Ghastly Grievance Passive Ability.");

      return false;
    }

    // Assign the references to the corresponding variables.
    playerStatusEffector ??= entityStatusEffectorRef;
    playerGhastlyGrievancePassive ??= entityStatusEffectorRef.TryGetStatusEffect<AspectOfFearPassiveAStatusEffectSO>();

    // Check the base criteria.
    return base.MeetsCriteria(progressionManager);
  }

  private protected override void OnActivated()
  {
    // Subscribe to the necessary Actions.
    playerGhastlyGrievancePassive.OnEntitySkulled += PlayerGhastlyGrievance_OnEntitySkulled;
    playerGhastlyGrievancePassive.OnSkulledEntityExecuted += SkulledEntity_OnExecution;
  }

  private protected override void OnCleanUp()
  {
    // Unsubscribe to any Actions used for the quest.
    playerGhastlyGrievancePassive.OnEntitySkulled -= PlayerGhastlyGrievance_OnEntitySkulled;
    playerGhastlyGrievancePassive.OnSkulledEntityExecuted -= SkulledEntity_OnExecution;
  }

  private protected override void OnUpdate() { }

  private void PlayerGhastlyGrievance_OnEntitySkulled(ExtendedDebuffStatusEffectSO ghastlyGrievance)
  {
    if(!appliedGhastlyGrievances.Contains(ghastlyGrievance))
      appliedGhastlyGrievances.Add(ghastlyGrievance);

    // Check if the updated successful hits and executions meet the goals.
    if (appliedGhastlyGrievances.Count >= SuccessfulHitsGoal && successfulExecutions >= SuccessfulExecutionsGoal)
      Complete();
  }

  private void SkulledEntity_OnExecution(ExtendedDebuffStatusEffectSO ghastlyGrievance)
  {
    if(playerGhastlyGrievancePassive.skulledEntities.ContainsValue(ghastlyGrievance) && appliedGhastlyGrievances.Contains(ghastlyGrievance))
      successfulExecutions++;

    // Check if the updated successful hits and executions meet the goals.
    if (appliedGhastlyGrievances.Count >= SuccessfulHitsGoal && successfulExecutions >= SuccessfulExecutionsGoal)
      Complete();
  }
}
