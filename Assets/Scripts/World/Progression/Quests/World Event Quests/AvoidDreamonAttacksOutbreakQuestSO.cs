using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Avoid_x_AttacksPrioritiesQuestSO", menuName = "World/Progression Quest/World Event Quests/Avoid Dreamon Attacks (Outbreak)")]
public class AvoidDreamonAttacksOutbreakQuestSO : WorldEventQuestSO
{
  [Header("Config")]
  [SerializeField] private List<string> enemyTypes;
  private ZonesWorldEventSO outbreakEventRef;
  private Player player;
  private bool attackedByDreamon;

  private protected override void OnActivated()
  {
    attackedByDreamon = false;
    player = progressionManager?.player;
    outbreakEventRef = (ZonesWorldEventSO)eventManager?.CurrentEvent;
    
    if (player == null || outbreakEventRef == null)
    {
      CleanUp();
      return;
    }

    player.OnEntityTakeDamage += Player_OnTakeDamage;
  }

  private void Player_OnTakeDamage(int damage, Vector3 hitPoint, GameObject source)
  {
    // If the player takes damage from an enemy whose type matches one in the enemyTypes list, the player fails the event.
    if (source.TryGetComponent(out Enemy enemy))
      if (enemyTypes.Contains(enemy.EnemyType))
        attackedByDreamon = true;
  }

  private protected override void OnCleanUp()
  {
    if (player == null || outbreakEventRef == null)
      return;

    if (attackedByDreamon == false)
      Complete(false);

    player.OnEntityTakeDamage -= Player_OnTakeDamage;
  }

  private protected override void OnUpdate()
  {
    Debug.Log($"Player attacked by Dreamon of specified type: {attackedByDreamon}");
  }
}
