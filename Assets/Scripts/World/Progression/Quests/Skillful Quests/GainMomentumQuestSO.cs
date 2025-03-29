using UnityEngine;

[CreateAssetMenu(fileName = "Gain_x_MomentumQuestSO", menuName = "World/Progression Quest/Skillful Quests/Gain Momentum")]
public class GainMomentumQuestSO : SkillfulQuestSO
{
  [Header("Config")]
  [Range(1, 100)]
  [SerializeField] private int minimumMomentum;

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
