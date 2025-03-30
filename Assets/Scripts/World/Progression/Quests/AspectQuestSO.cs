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
    if (requiredAspectTree == null || equippedAspectTrees == null)
      return false;

    AspectTree matchingTree = null;
    foreach (AspectTree tree in equippedAspectTrees)
    {
        if (tree != null && tree.GetType() == requiredAspectTree.GetType())
        {
            matchingTree = tree;
            break;
        }
    }
    
    if (matchingTree == null)
        return false;

    Debug.Log("Required ASPECT TREE Equipped");
    
    List<AspectNodeNode> aspectNodes = matchingTree.GetNodesAtLevel(nodeLevel);

    // Next, check if the required Aspect Node belongs to the required Aspect Tree. 
    if (requiredAspectNode == null) 
      return false;

    AspectNodeNode aspectNode = aspectNodes.Find(node => node.DisplayName == requiredAspectNode.DisplayName);
    if(aspectNode == null)
      return false;
    
    Debug.Log("Required ASPECT NODE belongs to Tree");

    // Finally, check if the required Aspect Node is already applied. If yes, then the game state meets the criteria for this quest.
    return aspectNode.IsApplied;
  } 
}
