using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectsManager : MonoBehaviour
{
    [SerializeField] private List<AspectTree> aspectTrees = new List<AspectTree>();

    public AspectTree CurrentAspectTree { get; private set; }
    [field: SerializeField] public int AspectTokens { get; private set; } = 10;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetCurrentAspectTree(aspectTrees[0]);
        }
    }

    public void SetCurrentAspectTree(AspectTree aspectTree)
    {
        CurrentAspectTree = aspectTree.CreateRuntimeInstance();

        Debug.Log($"Set current aspect tree to {CurrentAspectTree.name}");
    }

    public void ConsumeAspectToken()
    {
        AspectTokens--;
    }
}
