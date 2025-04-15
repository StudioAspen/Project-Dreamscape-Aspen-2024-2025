using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class AspectsManager : MonoBehaviour
{
    private LevelSystem levelSystem;

    [field: SerializeField] public List<AspectTree> AllAspectTrees { get; private set; } = new List<AspectTree>();
    public AspectTree[] EquippedAspectTrees { get; private set; } = new AspectTree[2];
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

    private void Awake()
    {
        levelSystem = GetComponent<LevelSystem>();
    }

    private void OnEnable()
    {
        levelSystem.OnLevelUp += LevelSystem_OnLevelUp;
    }
    private void OnDisable()
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
            AspectTokens++;
            Debug.LogWarning($"Cheat: Aspect Manager added 1 aspect token, total: {AspectTokens}");
        }
    }

    public AspectTree AddAspectTree(AspectTree newTree)
    {
        // Check to see if you already have the aspect type
        for (int i = 0; i < EquippedAspectTrees.Length; i++)
        {
            if (EquippedAspectTrees[i] == null) continue;
            if (EquippedAspectTrees[i].DisplayName == newTree.DisplayName)
            {
                Debug.LogWarning($"Can't add aspect tree {newTree.name}. You cannot have more than one of the same aspect.");
                return null;
            }
        }

        for (int i = 0; i < EquippedAspectTrees.Length; i++)
        {
            if (EquippedAspectTrees[i] != null) continue;
            AspectTree runtimeTree = newTree.CreateRuntimeInstance();
            EquippedAspectTrees[i] = runtimeTree;
            OnAspectTreeAdded.Invoke(runtimeTree);
            return runtimeTree;
        }

        Debug.LogWarning($"Can't add aspect tree {newTree.name}. You can only have up to 3 aspects.");
        return null;
    }

    public void ConsumeAspectToken()
    {
        AspectTokens--;
    }

    public List<AspectTree> GetAvailableNonEquippedAspects()
    {
        List<string> equippedAspectsDisplayNames = new();
        for(int i = 0; i < EquippedAspectTrees.Length; i++)
        {
            if (EquippedAspectTrees[i] == null) continue;
            equippedAspectsDisplayNames.Add(EquippedAspectTrees[i].DisplayName);
        }

        List<AspectTree> availableAspects = new List<AspectTree>(AllAspectTrees);
        foreach(AspectTree aspectTree in new List<AspectTree>(availableAspects))
        {
            if (equippedAspectsDisplayNames.Contains(aspectTree.DisplayName)) availableAspects.Remove(aspectTree);
        }

        return availableAspects;
    }

    public bool AreAllEquippedAspectsCompleted()
    {
        bool areAllAspectsCompleted = true;
        for (int i = 0; i < EquippedAspectTrees.Length; i++)
        {
            AspectTree tree = EquippedAspectTrees[i];
            if (tree == null)
            {
                areAllAspectsCompleted = false;
                break;
            }
            if (!tree.IsCompleted())
            {
                areAllAspectsCompleted = false;
                break;
            }
        }
        return areAllAspectsCompleted;
    }
}
