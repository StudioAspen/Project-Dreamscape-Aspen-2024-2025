using UnityEngine;

public abstract class WorldEventQuestSO : ProgressionQuestSO
{
  [field: Header("World Event Criteria")]
  /// <summary>
  /// The World Event required to be in effect for the Progression Manager to select this quest.
  /// </summary>
  [field: Tooltip("The World Event required to be in effect for the Progression Manager to select this quest.")]
  [field: SerializeField] protected WorldEventSO requiredWorldEvent;

  /// <summary>
  /// Reference to the Event Manager via the Progression Manager.
  /// </summary>
  protected EventManager eventManager;
}
