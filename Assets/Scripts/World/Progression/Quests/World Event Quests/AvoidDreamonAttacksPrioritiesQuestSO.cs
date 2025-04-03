using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Avoid_x_AttacksPrioritiesQuestSO", menuName = "World/Progression Quest/World Event Quests/Avoid Dreamon Attacks (Priorities)")]
public class AvoidDreamonAttacksPrioritiesQuestSO : WorldEventQuestSO
{
  [Header("Config")]
  [SerializeField] private List<Enemy> enemyTypes;
  private PrioritiesWorldEventSO prioritiesEventSO;
  private Player player;

  private protected override void OnActivated()
  {
    throw new System.NotImplementedException();
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
