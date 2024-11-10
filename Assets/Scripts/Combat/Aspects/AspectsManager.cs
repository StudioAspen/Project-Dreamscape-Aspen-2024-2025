using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectsManager : MonoBehaviour
{
    [SerializeField] private List<AspectTree> aspectTrees = new List<AspectTree>();

    private AspectTree currentAspectTree;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetCurrentAspectTree(aspectTrees[0]);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            ApplyRootAspect();
        }
    }

    public void SetCurrentAspectTree(AspectTree aspectTree)
    {
        currentAspectTree = aspectTree.Copy() as AspectTree;

        Debug.Log($"Set current aspect tree to {currentAspectTree.name}");
    }

    public void ApplyRootAspect()
    {
        AspectNodeNode currentNode = currentAspectTree.GetRootNode();

        Debug.Log($"Applying aspect {currentNode.name}");

        currentNode.ApplyAspect(this);
    }
}
