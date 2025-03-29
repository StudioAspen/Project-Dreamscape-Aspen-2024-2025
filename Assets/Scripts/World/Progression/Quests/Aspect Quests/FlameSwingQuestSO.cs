using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_TimesFlameSwingGroundedAspectQuestSO", menuName = "World/Progression Quest/Aspect Quests/Flame Swing (Grounded)")]

public class FlameSwingGroundedQuestSO : AspectQuestSO
{
  [Header("Config")]
  [Range(1, 10)]
  [SerializeField] private int minimumSuccessfulHits;

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
