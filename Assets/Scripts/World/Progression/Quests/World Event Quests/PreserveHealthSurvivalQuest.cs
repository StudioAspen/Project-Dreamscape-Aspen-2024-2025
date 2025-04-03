using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Preserve_x_PercentSurvivalQuestSO", menuName = "World/Progression Quest/World Event Quests/Preserve Health Percentage (Survival)")]
public class PreserveHealthSurvivalQuest : WorldEventQuestSO
{
  [Header("Config")]
  [Range(0.01f, 1.00f)]
  [SerializeField] private float minimumHealthPercentage;
  private SurvivalWorldEventSO survivalEventRef;
  private Player player;
  private int startingHealth = 0;

  private protected override void OnActivated()
  {
    survivalEventRef = (SurvivalWorldEventSO)eventManager?.CurrentEvent;
    player = progressionManager?.player;
    if (survivalEventRef == null || player == null)
    {
      CleanUp();
      return;
    }

    startingHealth = player.CurrentHealth;
  }

  private protected override void OnCleanUp()
  {
    if (survivalEventRef == null || player == null)
      return;

    if(player.CurrentHealth >= startingHealth * minimumHealthPercentage)
      Complete(false);
  }

  private protected override void OnUpdate()
  {
    Debug.Log($"Remaining Health: {player.CurrentHealth}, Minimum Required Health: {startingHealth * minimumHealthPercentage}");
  }
}
