using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_RemainingEscortQuestSO", menuName = "World/Progression Quest/World Event Quests/Remaining Health (Escort)")]
public class HealthRemainingEscortQuestSO : WorldEventQuestSO
{
  [Header("Config")]
  [Range(0.01f, 1.00f)]
  [SerializeField] private float minimumHealthPercentage;
  private EscortWorldEventSO escortEventRef;
  private EscortEventEntity escortEntity;

  private protected override void OnActivated()
  {
    escortEventRef = (EscortWorldEventSO)eventManager?.CurrentEvent;
    escortEntity = FindFirstObjectByType<EscortEventEntity>();
    if (escortEventRef == null || escortEntity == null)
    {
      CleanUp();
      return;
    }
  }

  private protected override void OnCleanUp()
  {
    if (escortEventRef == null || escortEntity == null)
      return;

    // Check for completion at the end of the wave.
    if (escortEntity.CurrentHealth >= escortEntity.MaxHealth.BaseValue * minimumHealthPercentage)
      Complete(false);
  }

  private protected override void OnUpdate()
  {
    Debug.Log($"Remaining Health: {escortEntity.CurrentHealth}, Minimum Required Health: {escortEntity.MaxHealth.BaseValue * minimumHealthPercentage}");
  }
}