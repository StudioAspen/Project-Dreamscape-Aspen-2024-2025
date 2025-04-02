using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_Times_y_ByGhastlyGrievanceQuestSO", menuName = "World/Progression Quest/Aspect Quests/Ghastly Grievance Execution")]
public class GhastlyGrievanceExecutionQuestSO : AspectQuestSO
{
  [field: Header("Config")]
  [field: Range(0, 15)]
  [field: SerializeField] public int SuccessfullySkulledEntitiesGoal { get; private set; }
  [field: Range(0, 5)]
  [field: SerializeField] public int SuccessfulExecutionsGoal { get; private set; }

  private Player player;
  private EntityStatusEffector playerStatusEffector;
  private AspectOfFearPassiveAStatusEffectSO playerGhastlyGrievance;
  private int successfulExecutions = 0;
  private List<ExtendedDebuffStatusEffectSO> successfullySkulledEntities = new List<ExtendedDebuffStatusEffectSO>();

  private protected override void OnActivated()
  {
    successfulExecutions = 0;
    successfullySkulledEntities.Clear();
    player = progressionManager?.player;

    if (player == null)
    {
      CleanUp();
      return;
    }

    playerStatusEffector = player.GetComponent<EntityStatusEffector>();
    playerGhastlyGrievance = playerStatusEffector?.TryGetStatusEffect<AspectOfFearPassiveAStatusEffectSO>();

    if(playerStatusEffector != null && playerGhastlyGrievance != null)
    {
      playerGhastlyGrievance.OnEntitySkulled += PlayerGhastlyGrievance_OnEntitySkulled;
      playerGhastlyGrievance.OnSkulledEntityExecuted += SkulledEntity_OnExecution;
    }
  }

  private protected override void OnCleanUp()
  {
    if(playerStatusEffector != null && playerGhastlyGrievance != null)
    {
      playerGhastlyGrievance.OnEntitySkulled -= PlayerGhastlyGrievance_OnEntitySkulled;
      playerGhastlyGrievance.OnSkulledEntityExecuted -= SkulledEntity_OnExecution;
    }
  }

  private protected override void OnUpdate()
  {
    Debug.Log($"Unique Skulled Entities: {successfullySkulledEntities.Count}, Executions: {successfulExecutions}");
    
    if (successfullySkulledEntities.Count >= SuccessfullySkulledEntitiesGoal && successfulExecutions >= SuccessfulExecutionsGoal)
      Complete();
  }

  private void PlayerGhastlyGrievance_OnEntitySkulled(ExtendedDebuffStatusEffectSO ghastlyGrievance)
  {
    if(!successfullySkulledEntities.Contains(ghastlyGrievance))
      successfullySkulledEntities.Add(ghastlyGrievance);
  }

  private void SkulledEntity_OnExecution(ExtendedDebuffStatusEffectSO ghastlyGrievance)
  {
    if(playerGhastlyGrievance.skulledEntities.ContainsValue(ghastlyGrievance) && successfullySkulledEntities.Contains(ghastlyGrievance))
      successfulExecutions++;
  }
}
