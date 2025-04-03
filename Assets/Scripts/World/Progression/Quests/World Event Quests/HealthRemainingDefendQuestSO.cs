using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_RemainingDefendQuestSO", menuName = "World/Progression Quest/World Event Quests/Remaining Health (Defend)")]
public class HealthRemainingDefendQuestSO : WorldEventQuestSO
{
  [Header("Config")]
  [Range(0.01f, 1.00f)]
  [SerializeField] private float minimumHealthPercentage;
  private DefendWorldEventSO defendEventRef;
  private DefendEventEntity defendEntity;

  private protected override void OnActivated()
  {
    defendEventRef = (DefendWorldEventSO)eventManager?.CurrentEvent;
    defendEntity = FindFirstObjectByType<DefendEventEntity>();
    if (defendEventRef == null || defendEntity == null)
    {
      CleanUp();
      return;
    }
  }

  private protected override void OnCleanUp()
  {
    if (defendEventRef == null || defendEntity == null)
      return;

    // Check for completion at the end of the wave.
    if (defendEntity.CurrentHealth >= defendEntity.MaxHealth.BaseValue * minimumHealthPercentage)
      Complete(false);
  }

  private protected override void OnUpdate()
  {
    Debug.Log($"Remaining Health: {defendEntity.CurrentHealth}, Minimum Required Health: {defendEntity.MaxHealth.BaseValue * minimumHealthPercentage}");
  }
}