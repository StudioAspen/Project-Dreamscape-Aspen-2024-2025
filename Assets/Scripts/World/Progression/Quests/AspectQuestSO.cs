using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AspectQuestSO : ProgressionQuestSO
{
  [Header("Aspect Criteria")]
  [SerializeField] protected AspectTree requiredAspectTree;
  [SerializeField] protected AspectNodeNode requiredAspectNode;
  [Range(0, 5)]
  [SerializeField] protected int nodeLevel;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    AspectTree[] equippedAspectTrees = progressionManager.aspectsManager.EquippedAspectTrees;

    // First, check if the player has equipped the required Aspect Tree.
    if (!equippedAspectTrees.Contains(requiredAspectTree) || requiredAspectTree == null)
      return false;

    AspectTree aspectTree = Array.Find(equippedAspectTrees, tree => tree == requiredAspectTree);
    
    List<AspectNodeNode> aspectNodes = aspectTree.GetNodesAtLevel(nodeLevel);

    // Next, check if the required Aspect Node belongs to the required Aspect Tree. 
    if (!aspectNodes.Contains(requiredAspectNode) || requiredAspectNode == null)
      return false;
      
    AspectNodeNode aspectNode = aspectNodes.Find(node => node == requiredAspectNode);
    
    // Finally, check if the required Aspect Node is already applied. If yes, then the game state meets the criteria for this quest.
    return aspectNode.IsApplied;
  } 
}
