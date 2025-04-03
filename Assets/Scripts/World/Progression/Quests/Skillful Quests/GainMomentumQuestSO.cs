using UnityEngine;

[CreateAssetMenu(fileName = "Gain_x_MomentumQuestSO", menuName = "World/Progression Quest/Skillful Quests/Gain Momentum")]
public class GainMomentumQuestSO : SkillfulQuestSO
{
  [field: Header("Config")]
  [field: Range(1, 100)]
  [field: SerializeField] public int MomentumGoal { get; private set; }

  private MomentumSystem momentumSystem;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    momentumSystem = FindObjectOfType<MomentumSystem>();
    
    if(momentumSystem == null)
      return false;
    else if (momentumSystem.Momentum >= MomentumGoal)
      return false;
      
    return base.MeetsCriteria(progressionManager);
  }
  private protected override void OnActivated()
  {
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
