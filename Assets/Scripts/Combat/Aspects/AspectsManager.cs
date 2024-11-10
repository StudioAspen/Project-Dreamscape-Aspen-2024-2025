using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectsManager : MonoBehaviour
{
    [SerializeField] private List<AspectTree> aspectTrees = new List<AspectTree>();

    [field:SerializeField] public AspectTree CurrentAspectTree { get; private set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetCurrentAspectTree(aspectTrees[0]);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            ApplyNextFirstAspectNode();
        }
    }

    public void SetCurrentAspectTree(AspectTree aspectTree)
    {
        CurrentAspectTree = aspectTree.CreateRuntimeInstance();

        Debug.Log($"Set current aspect tree to {CurrentAspectTree.name}");
    }

    public void ApplyNextFirstAspectNode()
    {
        List<AspectNodeNode> targetNodes = CurrentAspectTree.GetNextUnappliedNodes();
        if(targetNodes.Count == 0)
        {
            Debug.Log("No more nodes to apply");
            return;
        }

        AspectNodeNode targetNode = targetNodes[0];

        Debug.Log($"Applying aspect {targetNode.name}");

        targetNode.ApplyAspect(this);
    }
}
