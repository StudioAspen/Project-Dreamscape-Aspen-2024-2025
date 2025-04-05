using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AspectQuestSO : ProgressionQuestSO
{
  [Header("Aspect Criteria")]

  /// <summary>
  /// The Aspect Tree required to be equipped for the Progression Manager to select this quest.
  /// </summary>
  [Tooltip("The Aspect Tree required to be equipped for the Progression Manager to select this quest.")]
  [SerializeField] protected AspectTree requiredAspectTree;

  /// <summary>
  /// The Aspect Node required to be applied for the Progression Manager to select this quest. This Aspect Node must also belong to the required Aspect Tree, and the player has to have applied it.
  /// </summary>
  [Tooltip("The Aspect Node required to be applied for the Progression Manager to select this quest. This Aspect Node must also belong to the Required Aspect Tree, and the player has to have applied it.")]
  [SerializeField] protected AspectNodeNode requiredAspectNode;

  /// <summary>
  /// The Node Level at which the Progression Manager will search the required Aspect Tree for the required Aspect Node.
  /// </summary>
  [Tooltip("The Node Level at which the Progression Manager will search the required Aspect Tree for the required Aspect Node.")]
  [Range(0, 5)]
  [SerializeField] protected int nodeLevel;

  /// <summary>
  /// Reference to the equipped Aspect Trees via the Progression Manager and Aspects Manager.
  /// </summary>
  protected AspectTree[] equippedAspectTrees;

  public override bool MeetsCriteria(ProgressionManager progressionManager)
  {
    // First, check if the player has equipped the required Aspect Tree.
    if (requiredAspectTree == null)
    {
      if (LogErrorMessages)
        Debug.LogError("Quest Criteria Error: A required Aspect Tree was not provided.");

      return false;
    }
    else if (progressionManager.aspectsManager.EquippedAspectTrees == null)
    {
      if (LogErrorMessages)
        Debug.LogError("Quest Criteria Error: Could not find reference to Equipped Aspect Trees.");

      return false;
    }

    // Assign the equipped Aspect Trees reference to the variable.
    equippedAspectTrees ??= progressionManager.aspectsManager.EquippedAspectTrees;

    // Search the equipped Aspect Trees for one that matches the type of the required Aspect Tree.
    AspectTree matchingTree = null;
    foreach (AspectTree tree in equippedAspectTrees)
    {
      if (tree != null && tree.GetType() == requiredAspectTree.GetType())
      {
        // Assign the matching Aspect Tree reference to the variable.
        matchingTree = tree;
        break;
      }
    }
    
    // If Matching Tree is null, it means that the player has no Aspect Trees equipped (i.e. EquippedAspectTrees.Length == 0)
    if (matchingTree == null)
    {
      if (LogErrorMessages)
        Debug.LogError("Quest Criteria Error: Player does not have any Aspect Trees equipped.");

      return false;
    }

    // Get the Aspect Nodes at the specified Node Level and search for the required Aspect Node among them.
    List<AspectNodeNode> aspectNodes = matchingTree.GetNodesAtLevel(nodeLevel);
    AspectNodeNode aspectNode = aspectNodes.Find(node => node.DisplayName == requiredAspectNode.DisplayName);

    // Next, check if the required Aspect Node belongs to the required Aspect Tree. 
    if (requiredAspectNode == null) 
    {
      if (LogErrorMessages)
        Debug.LogError("Quest Criteria Error: A required Aspect Node was not provided.");

      return false;
    }
    else if(aspectNode == null)
    {
      if (LogErrorMessages)
        Debug.LogError("Quest Criteria Error: Required Aspect Node was not found at the specified Node Level.");

      return false;
    }

    // Finally, check if the required Aspect Node is already applied. If yes, then the game state meets the criteria for this quest.
    return aspectNode.IsApplied;
  } 
}
