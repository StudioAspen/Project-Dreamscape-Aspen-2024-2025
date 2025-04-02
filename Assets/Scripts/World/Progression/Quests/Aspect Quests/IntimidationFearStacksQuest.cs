using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_FearStacksByIntimidationQuestSO", menuName = "World/Progression Quest/Aspect Quests/Indimidation Fear Stacks")]
public class IntimidationFearStacksQuest : AspectQuestSO
{
  [field: Header("Config")]
  [field: Range(0, 5)]
  [field: SerializeField] public int FearStacksGoal { get; private set; }
  private Player player;
  private EntityStatusEffector playerStatusEffector;
  private AspectOfFearPassiveBStatusEffectSO playerIntimidation;
  private int fearStacks = 0;

  private protected override void OnActivated()
  {
    player = progressionManager?.player;

    if (player == null)
    {
      CleanUp();
      return;
    }

    playerStatusEffector = player.GetComponent<EntityStatusEffector>();
    playerIntimidation = playerStatusEffector?.TryGetStatusEffect<AspectOfFearPassiveBStatusEffectSO>();

    if (playerStatusEffector != null && playerIntimidation != null)
    {
      fearStacks = playerIntimidation.CurrentStacks;
      playerIntimidation.OnStunEntity += PlayerIntimidation_OnEntityStunned;
    }
  }


  private protected override void OnCleanUp()
  {
    if (playerStatusEffector != null && playerIntimidation != null)
    {
      fearStacks = 0;
      playerIntimidation.OnStunEntity -= PlayerIntimidation_OnEntityStunned;
    }
  }

  private protected override void OnUpdate()
  {
    if (fearStacks >= FearStacksGoal)
      Complete();
  }

  private void PlayerIntimidation_OnEntityStunned(Entity stunner, Entity victim, float stunDuration)
  {
    fearStacks = playerIntimidation.CurrentStacks;
    Debug.Log($"Current tracked Fear Stacks: {fearStacks}");
  }
}
