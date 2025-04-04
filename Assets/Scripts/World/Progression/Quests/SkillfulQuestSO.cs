using UnityEngine;

public abstract class SkillfulQuestSO : ProgressionQuestSO
{
  [Header ("Skillful Criteria")]
  /// <summary>
  /// The minimum player level required for the Progression Manager to select this quest.
  /// </summary>
  [Tooltip("The minimum player level required for the Progression Manager to select this quest.")]
  [Range(1, 99)]
  [SerializeField] protected int requiredPlayerLevel;

  /// <summary>
  /// Reference to the Event Manager via the Progression Manager.
  /// </summary>
  protected LevelSystem levelSystem;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  { 
    if (!progressionManager.levelSystem)
    {
      if (LogErrorMessages)
        Debug.LogError($"{name} Criteria Error: Could not find reference to the Level System.");

      return false;
    }

    // Assign the Level System reference to the variable.
    levelSystem ??= progressionManager.levelSystem;
    
    // Check whether the player is at least the required Level.
    return levelSystem.Level >= requiredPlayerLevel;
  }
}
