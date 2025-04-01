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
  private PlayerCombat playerCombat; 
  private AspectOfFearPassiveAStatusEffectSO playerGhastlyGrievance;
  // Extended Debuff = Ghastly Grievance
  private HashSet<Entity> skulledEntities = new HashSet<Entity>();

  private int successfulExecutions = 0;

  private protected override void OnActivated()
  {
    successfulExecutions = 0;

  }

  private protected override void OnCleanUp()
  {
    throw new System.NotImplementedException();
  }

  private protected override void OnUpdate()
  {
    throw new System.NotImplementedException();
  }
}
