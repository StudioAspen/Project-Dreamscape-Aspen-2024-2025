using UnityEngine;

public abstract class SkillfulQuestSO : ProgressionQuestSO
{
  [Header ("Skillful Criteria")]
  [Range(1, 99)]
  [SerializeField] protected int requiredPlayerLevel;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {  
    // Check whether the player is at least the required Level.
    if (progressionManager.levelSystem.Level < requiredPlayerLevel)
      return false;

    return true;
  }
}
