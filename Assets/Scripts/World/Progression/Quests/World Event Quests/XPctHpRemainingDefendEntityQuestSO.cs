using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_PctHpRemainingDefendEntityQuestSO", menuName = "World/Progression Quest/World Event Quests/Remaining Defend Entity HP")]
public class XPctRemainingDefendEntityQuestSO : WorldEventQuestSO
{
  [Header("Config")]
  [Range(0.01f, 1.00f)]
  [SerializeField] private float minimumHealthPercentage; 

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