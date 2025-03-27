using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_x_TimesFlameSwingAspectQuestSO", menuName = "World/Progression Quest/Aspect Quests/Flame Swing")]

public class FlameSwingQuestSO : AspectQuestSO
{
  [Header("Config")]
  [Range(1, 10)]
  [SerializeField] private int minimumSuccessfulHits;

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
