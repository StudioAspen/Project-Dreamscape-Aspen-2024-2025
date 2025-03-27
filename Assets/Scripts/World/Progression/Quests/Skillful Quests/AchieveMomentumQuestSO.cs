using UnityEngine;

[CreateAssetMenu(fileName = "Achieve_x_MomentumQuestSO", menuName = "World/Progression Quest/Skillful Quests/Achieve Momentum")]
public class AchieveMomentumQuestSO : SkillfulQuestSO
{
  [Header("Config")]
  [Range(1, 100)]
  [SerializeField] private int minimumMomentum;

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
