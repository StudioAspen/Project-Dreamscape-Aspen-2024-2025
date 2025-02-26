using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectsManager : MonoBehaviour
{
    private LevelSystem levelSystem;

    [field: SerializeField] public List<AspectTree> AllAspectTrees { get; private set; } = new List<AspectTree>();
    public AspectTree[] EquippedAspectTrees { get; private set; } = new AspectTree[3];
    public AspectTree CurrentAspectTree { get; private set; }
    public int AspectTokens { get; private set; } = 0;

    /// <summary>
    /// Action that is invoked when an aspect is added
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>AspectTree aspectTree</c>: The added aspect tree</description></item>
    /// </list>
    /// </remarks>
    public Action<AspectTree> OnAspectTreeAdded = delegate { };

    private void Start()
    {
        levelSystem = GetComponent<LevelSystem>();

        levelSystem.OnLevelUp += LevelSystem_OnLevelUp;

        SetCurrentAspectTree(AllAspectTrees[0]);
        AddAspectTree(AllAspectTrees[1]);
        AddAspectTree(AllAspectTrees[2]);
    }

    private void OnDestroy()
    {
        levelSystem.OnLevelUp -= LevelSystem_OnLevelUp;
    }

    private void LevelSystem_OnLevelUp(int newLevel)
    {
        AspectTokens++;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.LogWarning($"Cheat: Aspect Manager added 1 aspect token, total: {AspectTokens}");
            AspectTokens++;
        }
    }

    /// <summary>
    /// Sets the current aspect tree to the specified runtime instance of the aspect tree.
    /// </summary>
    /// <param name="aspectTree">The aspect tree to set as the current aspect tree.</param>
    public void SetCurrentAspectTree(AspectTree aspectTree)
    {
        CurrentAspectTree = aspectTree.CreateRuntimeInstance();

        AddAspectTree(aspectTree);

        //Debug.Log($"Set current aspect tree to {CurrentAspectTree.name}");
    }

    public void AddAspectTree(AspectTree newTree)
    {
        for (int i = 0; i < EquippedAspectTrees.Length; i++)
        {
            if (EquippedAspectTrees[i] != null) continue;

            AspectTree runtimeTree = newTree.CreateRuntimeInstance();
            EquippedAspectTrees[i] = runtimeTree;
            OnAspectTreeAdded.Invoke(runtimeTree);

            return;
        }

        Debug.LogWarning($"Can't add aspect tree {newTree.name}. You can only have up to 3 aspects.");
    }

    public void ConsumeAspectToken()
    {
        AspectTokens--;
    }
}
