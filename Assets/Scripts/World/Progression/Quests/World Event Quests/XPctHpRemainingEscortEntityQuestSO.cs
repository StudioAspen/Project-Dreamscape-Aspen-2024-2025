using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_PctHpRemainingEscortEntityQuestSO", menuName = "World/Progression Quest/World Event Quests/Remaining Escort Entity HP")]
public class XPctRemainingEscortEntityQuestSO : WorldEventQuestSO
{
  [Header("Config")]
  [Range(0.01f, 1.00f)]
  [SerializeField] private float minimumHealthPercentage; 

  private protected override void OnActivated()
  {
    
  }

  private protected override void OnCleanUp()
  {
    
  }

  private protected override void OnUpdate()
  {
    
  }
}
