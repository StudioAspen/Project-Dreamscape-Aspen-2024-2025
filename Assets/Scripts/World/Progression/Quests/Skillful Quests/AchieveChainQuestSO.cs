using UnityEngine;

[CreateAssetMenu(fileName = "Achieve_x_ChainQuestSO", menuName = "World/Progression Quest/Skillful Quests/Achieve Chain")]
public class AchieveChainQuestSO : SkillfulQuestSO
{
  [field: Header("Config")]
  [field: Range(1, 100)]
  [field: SerializeField] public int ChainGoal { get; private set; }

  private ChainingSystem chainingSystem;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    chainingSystem = FindFirstObjectByType<ChainingSystem>();

    if (chainingSystem == null)
      return false;
    else if (chainingSystem.ChainCount >= ChainGoal)
      return false;

    return base.MeetsCriteria(progressionManager);
  }
  private protected override void OnActivated()
  {
    if (chainingSystem == null)
      CleanUp();
  }

  private protected override void OnCleanUp()
  {

  }

  private protected override void OnUpdate()
  {
    if (chainingSystem == null || IsCompleted)
      return;

    if (chainingSystem.ChainCount >= ChainGoal)
      Complete();
  }
}
