using UnityEngine;

[CreateAssetMenu(fileName = "Gain_x_MomentumQuestSO", menuName = "World/Progression Quest/Skillful Quests/Gain Momentum")]
public class GainMomentumQuestSO : SkillfulQuestSO
{
  [field: Header("Config")]
  [field: Range(1, 100)]
  [field: SerializeField] public int MomentumGoal { get; private set; }

  private MomentumSystem momentumSystem;

  private protected override void OnActivated()
  {
    momentumSystem = FindObjectOfType<MomentumSystem>();

    if (momentumSystem == null)
      CleanUp();
  }

  private protected override void OnCleanUp()
  {

  }

  private protected override void OnUpdate()
  {
    if (momentumSystem == null || IsCompleted)
      return;

    if (momentumSystem.Momentum >= MomentumGoal)
      Complete();
  }
}
