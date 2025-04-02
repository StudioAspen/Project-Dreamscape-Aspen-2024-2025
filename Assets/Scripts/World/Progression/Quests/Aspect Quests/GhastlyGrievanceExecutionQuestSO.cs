using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_TimesExecutionQuestSO", menuName = "World/Progression Quest/Aspect Quests/Ghastly Grievance Execution")]
public class GhastlyGrievanceExecutionQuestSO : AspectQuestSO
{
  [field: Header("Config")]
  [field: Range(1, 5)]
  [field: SerializeField] public int SuccessfulExecutionsGoal { get; private set; }

  private Player player;
  private EntityStatusEffector playerStatusEffector;
  private AspectOfFearPassiveAStatusEffectSO playerGhastlyGrievance;
  private int successfulExecutions = 0;

  private protected override void OnActivated()
  {
    successfulExecutions = 0;
    player = progressionManager?.player;

    if (player == null)
      CleanUp();

    playerStatusEffector = player.GetComponent<EntityStatusEffector>();
    playerGhastlyGrievance = playerStatusEffector?.TryGetStatusEffect<AspectOfFearPassiveAStatusEffectSO>();

    if(playerStatusEffector != null && playerGhastlyGrievance != null)
    {
      playerGhastlyGrievance.OnSkulledEntityExecuted += SkulledEntity_OnExecution;
    }
  }

  private protected override void OnCleanUp()
  {
    if(playerStatusEffector != null && playerGhastlyGrievance != null)
    {
      playerGhastlyGrievance.OnSkulledEntityExecuted -= SkulledEntity_OnExecution;
    }
  }

  private protected override void OnUpdate()
  {
    if (successfulExecutions >= SuccessfulExecutionsGoal)
      Complete();
  }

  private void SkulledEntity_OnExecution(ExtendedDebuffStatusEffectSO ghastlyGrievance)
  {
    if(playerGhastlyGrievance.skulledEntities.ContainsValue(ghastlyGrievance))
      successfulExecutions++;
  }
}
