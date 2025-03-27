using UnityEngine;

public abstract class SkillfulQuestSO : ProgressionQuestSO
{
  [Header ("Skillful Criteria")]
  [Range(0, 99)]
  [SerializeField] protected int requiredPlayerLevel;
  [Range(0, 99)]
  [SerializeField] protected int requiredWaveIndex;

  public override bool MeetsCriteria()
  {
    LevelSystem levelSystem = progressionManager.levelSystem;

    // First, check whether the player is at least the required Level, if greater than 0.
    if (levelSystem.Level < requiredPlayerLevel)
      return false;

    // First, check whether the current run is at least the required Wave Index, if greater than 0.
    if (progressionManager.WaveIndex < requiredWaveIndex)
      return false;

    return true;
  }
}
