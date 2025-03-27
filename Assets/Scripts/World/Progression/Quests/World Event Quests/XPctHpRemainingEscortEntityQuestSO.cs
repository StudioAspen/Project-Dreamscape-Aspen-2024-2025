using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_PctHpRemainingEscortEntityQuestSO", menuName = "World/Progression Quest/World Event Quests/Remaining Escort Entity HP")]
public class XPctRemainingEscortEntityQuestSO : WorldEventQuestSO
{
  [Header("Config")]
  [Range(0.01f, 1.00f)]
  [SerializeField] private float minimumHealthPercentage; 

  public override bool MeetsCriteria()
  {
    return base.MeetsCriteria();
  }

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
