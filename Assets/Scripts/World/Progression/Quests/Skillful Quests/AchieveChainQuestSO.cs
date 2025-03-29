using UnityEngine;

[CreateAssetMenu(fileName = "Achieve_x_ChainQuestSO", menuName = "World/Progression Quest/Skillful Quests/Achieve Chain")]
public class AchieveChainQuestSO : SkillfulQuestSO
{
  [Header("Config")]
  [Range(1, 100)]
  [SerializeField] private int minimumChain;

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
